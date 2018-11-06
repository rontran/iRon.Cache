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

    public class CacheRepository<T> : ICacheRepository<T>
        where T : class
    {
        private readonly TimeSpan defaultTimeSpan;
        private readonly ICacheConfig cacheConfig;
        private StackExchangeRedisCacheClient client;
        private IConnectionMultiplexer connectionMultiplexer;

        public CacheRepository(ICacheConfig cacheConfig)
        {
            this.cacheConfig = cacheConfig;
            this.connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConfig.ConnectionString);
            var settings = new JsonSerializerSettings
            {
                // ContractResolver = new CustomResolver()
            };
            settings.Converters.Add(new BsonNullConverter());
            var serializer = new NewtonsoftSerializer(settings);
            this.client = new StackExchangeRedisCacheClient(this.connectionMultiplexer, serializer, 0, cacheConfig.Prefix);
            this.defaultTimeSpan = new TimeSpan(0, 0, 0);
        }

        public void Delete(string key)
        {
            this.client.Remove(key);
        }

        public void DeleteAsync(string key)
        {
            this.client.RemoveAsync(key);
        }

        public void DeletePattern(string key)
        {
            this.client.RemoveAll(this.client.SearchKeys(key));

        }

        public void DeletePatternAsync(string key)
        {
            this.client.RemoveAllAsync(this.client.SearchKeys(key));
        }

        public bool Exists(string key)
        {
            return this.client.Exists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return this.client.ExistsAsync(key);
        }

        public void FlushAll()
        {
            this.client.FlushDb();
        }

        public void FlushAllAsync()
        {
            this.client.FlushDbAsync();
        }

        public T Get(string key)
        {
            return this.client.Get<T>(key);
        }

        public Task<T> GetAsync(string key)
        {
            return this.client.GetAsync<T>(key);
        }

        public IEnumerable<T> Gets(string key)
        {
            return this.client.Get<IEnumerable<T>>(key);
        }

        public Task<IEnumerable<T>> GetsAsync(string key)
        {
            return this.client.GetAsync<IEnumerable<T>>(key);
        }

        public void Set(string key, T entity)
        {
            var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
            var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
            this.client.Add<T>(key, entity, timeSpan);
        }

        public void SetAsync(string key, T entity)
        {
            var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
            var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
            this.client.AddAsync<T>(key, entity, timeSpan);
        }

        public void Set(string key, IEnumerable<T> entity)
        {
            var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
            var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
            this.client.Add<IEnumerable<T>>(key, entity, timeSpan);
        }

        public void SetAsync(string key, IEnumerable<T> entity)
        {
            var attr = typeof(T).GetCustomAttribute<CacheDuration>(false);
            var timeSpan = attr == null ? this.defaultTimeSpan : new TimeSpan(0, 0, this.GetDuration(attr.Duration));
            this.client.AddAsync<IEnumerable<T>>(key, entity, timeSpan);
        }

        public bool IsConnected()
        {
            return this.connectionMultiplexer.IsConnected;
        }

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
