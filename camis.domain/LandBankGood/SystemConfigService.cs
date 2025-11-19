using intapscamis.camis.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace intapscamis.camis.domain.LandBank;

public interface ISystemConfigService
{
    Task<string> GetBaseUrlAsync();
}
public class SystemConfigService(CamisContext context, IMemoryCache cache) : ISystemConfigService
{
    private const string CacheKey = "NRLAIS_url";

    public async Task<string> GetBaseUrlAsync()
    {
        // Try to get from cache first
        if (cache.TryGetValue(CacheKey, out string baseUrl))
            return baseUrl;

        // Get from database
        baseUrl = await context.SysConfigs
            .Where(x => x.Name == "NRLAIS_url")
            .Select(x => x.Value)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(baseUrl))
            throw new InvalidOperationException("NRLAIS BaseUrl is not configured in database");

        // Cache for 1 hour
        cache.Set(CacheKey, baseUrl, TimeSpan.FromHours(1));
        return baseUrl;
    }
}