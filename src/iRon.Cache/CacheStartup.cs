
using iRon.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace iRon.Cache
{
    public static class CacheStartup
    {
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection UseCache(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            services.Configure<CacheConfig>(config.GetSection("iRon.Cache"));
            services.AddSingleton(typeof(ICacheRepository<>),typeof(CacheRepository<>));
            return services;
        }
    }
}
