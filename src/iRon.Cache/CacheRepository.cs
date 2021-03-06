﻿namespace iRon.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using StackExchange.Redis.Extensions.Core;
    using StackExchange.Redis.Extensions.Newtonsoft;
    using iRon.Cache.Attributes;
    using iRon.Cache.Enums;
    using iRon.Cache.Interfaces;
    using iRon.Cache.MongoDb;
    using Microsoft.Extensions.Options;

    public class CacheRepository<T> : ICacheRepository<T>
        where T : class
    {
        private readonly TimeSpan defaultTimeSpan;
        private readonly CacheConfig cacheConfig;
        private readonly StackExchangeRedisCacheClient client;
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public CacheRepository(IOptions<CacheConfig> cacheConfig)
        {
            this.cacheConfig = cacheConfig.Value;

            if (this.cacheConfig.Enabled)
            {
                this.connectionMultiplexer = ConnectionMultiplexer.Connect(this.cacheConfig.ConnectionString);
                var settings = new JsonSerializerSettings
                {
                    // ContractResolver = new CustomResolver()
                };
                settings.Converters.Add(new BsonNullConverter());
                var serializer = new NewtonsoftSerializer(settings);
                this.client = new StackExchangeRedisCacheClient(this.connectionMultiplexer, serializer, 0, this.cacheConfig.Prefix);
            }
            this.defaultTimeSpan = new TimeSpan(0, 0, 0);
        }

        
        public void DeleteAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.RemoveAsync(key);
        }

        
        public void DeletePatternAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                this.client.RemoveAllAsync(this.client.SearchKeys(key));
            }
        }

        public Task<IEnumerable<string>> GetKeysAsync(string pattern)
        {
            return this.client.SearchKeysAsync(pattern);
        }
        
        public Task<bool> ExistsAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) { return this.client.ExistsAsync(key); }
            else
            {
                return Task.FromResult<bool>(false);
            }
        }
                
        public void FlushAllAsync()
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.FlushDbAsync();
        }

        public Task<T> GetAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                return this.client.GetAsync<T>(key);
            }
            else
            {
                return Task.FromResult<T>(default(T));
            }
        }

        public Task<IEnumerable<T>> GetsAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                return this.client.GetAsync<IEnumerable<T>>(key);
            }
            else
            {
                return Task.FromResult<IEnumerable<T>>(default(IEnumerable<T>));
            }
        }

        public void SetAsync(string key, T entity, Duration? overrideDuration = null)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
                if (attr != null && attr.Duration == Duration.NONE) return;
                var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
                if (overrideDuration != null) timeSpan = new TimeSpan(0, 0, this.GetDuration(overrideDuration.GetValueOrDefault()));
                this.client.AddAsync<T>(key, entity, timeSpan);
            }
        }

        public void SetAsync(string key, IEnumerable<T> entity, Duration? overrideDuration = null)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
                if (attr != null && attr.Duration == Duration.NONE) return;
                var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
                if (overrideDuration != null) timeSpan = new TimeSpan(0, 0, this.GetDuration(overrideDuration.GetValueOrDefault()));
                this.client.AddAsync<IEnumerable<T>>(key, entity, timeSpan);
            }
        }

        public bool IsConnected => connectionMultiplexer != null && this.connectionMultiplexer.IsConnected;

        private int GetDuration(Duration duration)
        {
            switch (duration)
            {
                case Duration.NONE:
                    return this.cacheConfig.Duration.None;
                case Duration.LOW:
                    return this.cacheConfig.Duration.Low;
                case Duration.NORMAL:
                    return this.cacheConfig.Duration.Normal;
                case Duration.HIGH:
                    return this.cacheConfig.Duration.High;
                case Duration.FOREVER:
                    return this.cacheConfig.Duration.Forever;
                default:
                    return 0;
            }
        }
    }
}
