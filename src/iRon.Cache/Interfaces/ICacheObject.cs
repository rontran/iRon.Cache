using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iRon.Cache.Interfaces
{
    public interface ICacheRepository<T> where T:class
    {
        void SetAsync(string key, IEnumerable<T> entity);
        void Set(string key, IEnumerable<T> entity);
        IEnumerable<T> Gets(string key);
        void Set(string key, T entity);
        void SetAsync(string key, T entity);
        bool IsConnected { get; }
        T Get(string key);
        Task<T> GetAsync(string key);
        Task<IEnumerable<T>> GetsAsync(string key);
        void FlushAll();
        void FlushAllAsync();
        bool Exists(string key);
        Task<bool> ExistsAsync(string key);
        void Delete(string key);
        void DeleteAsync(string key);
        void DeletePattern(string key);
        void DeletePatternAsync(string key);

    }
}
