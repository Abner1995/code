using AspNetCoreRateLimit;

namespace Learn.WebAPI.StartupConfig;

public static class ServicesConfig
{
    public static void AddRateLimitServices(this WebApplicationBuilder builder1)
    {
        builder1.Services.Configure<IpRateLimitOptions>(
        builder1.Configuration.GetSection("IpRateLimiting"));
        builder1.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        builder1.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        builder1.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        builder1.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        builder1.Services.AddInMemoryRateLimiting();
    }
}
