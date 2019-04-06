using iRon.Cache.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iRon.Cache.Interfaces
{
    public interface ICacheRepository<T> where T : class
    {
        void SetAsync(string key, IEnumerable<T> entity, Duration? overrideDuration = null);
        void SetAsync(string key, T entity, Duration? overrideDuration = null);
        bool IsConnected { get; }
        Task<T> GetAsync(string key);
        Task<IEnumerable<T>> GetsAsync(string key);
        void FlushAllAsync();
        Task<bool> ExistsAsync(string key);
        void DeleteAsync(string key);
        void DeletePatternAsync(string key);
        Task<IEnumerable<string>> GetKeysAsync(string pattern);
    }
}
