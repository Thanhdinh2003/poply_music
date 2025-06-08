namespace WebApi.Configurations
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddCacheConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var typeOfCacheSystem = configuration.GetValue<string>("CacheSettings:Type");
            switch (typeOfCacheSystem)
            {
                case "redis":
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }
            return services;
        }
    }
}
