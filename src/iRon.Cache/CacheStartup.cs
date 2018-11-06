using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iRon.Cache.Interfaces;

namespace iRon.Cache
{
    public static class CacheStartup
    {
        public static IServiceCollection UseCache(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CacheConfig>(config.GetSection("Caching").Value, null);
            services.AddSingleton(typeof(ICacheRepository<>),typeof(CacheRepository<>));
            return services;
        }
    }
}
