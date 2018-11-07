namespace iRon.Cache
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
            this.connectionMultiplexer = ConnectionMultiplexer.Connect(this.cacheConfig.ConnectionString);
            var settings = new JsonSerializerSettings
            {
                // ContractResolver = new CustomResolver()
            };
            settings.Converters.Add(new BsonNullConverter());
            var serializer = new NewtonsoftSerializer(settings);
            if (this.cacheConfig.Enabled)
            {
                this.client = new StackExchangeRedisCacheClient(this.connectionMultiplexer, serializer, 0, this.cacheConfig.Prefix);
            }
            this.defaultTimeSpan = new TimeSpan(0, 0, 0);
        }

        public void Delete(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.Remove(key);
        }

        public void DeleteAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.RemoveAsync(key);
        }

        public void DeletePattern(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.RemoveAll(this.client.SearchKeys(key));

        }

        public void DeletePatternAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.RemoveAllAsync(this.client.SearchKeys(key));
        }

        public bool Exists(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) { return this.client.Exists(key); }
            else
            { return false; }
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) { return this.client.ExistsAsync(key); }
            else
            {
                return Task.FromResult<bool>(false);
            }
        }

        public void FlushAll()
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.FlushDb();
        }

        public void FlushAllAsync()
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) this.client.FlushDbAsync();
        }

        public T Get(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected) { return this.client.Get<T>(key); }
            else
            {
                return default(T);
            }
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

        public IEnumerable<T> Gets(string key)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                return this.client.Get<IEnumerable<T>>(key);
            }
            else
            {
                return default(IEnumerable<T>);
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

        public void Set(string key, T entity)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
                if (attr != null && attr.Duration == Duration.NONE) return;
                var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
                this.client.Add<T>(key, entity, timeSpan);
            }
        }

        public void SetAsync(string key, T entity)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
                if (attr != null && attr.Duration == Duration.NONE) return;
                var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
                this.client.AddAsync<T>(key, entity, timeSpan);
            }
        }

        public void Set(string key, IEnumerable<T> entity)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
                if (attr != null && attr.Duration == Duration.NONE) return;
                var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
                this.client.Add<IEnumerable<T>>(key, entity, timeSpan);
            }
        }

        public void SetAsync(string key, IEnumerable<T> entity)
        {
            if (client != null && this.cacheConfig.Enabled && IsConnected)
            {
                var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
                if (attr != null && attr.Duration == Duration.NONE) return;
                var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
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
