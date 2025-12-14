using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace intapscamis.camis.domain.EnvironmentalMonitoring
{
    public interface ISatelliteServiceFactory
    {
        ISatelliteImageryService GetSatelliteService(bool useRealService = false);
    }
    public class SatelliteServiceFactory:ISatelliteServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SentinelHubService> _sentinelLogger;
        private readonly ILogger<SatelliteImageryService> _simulatedLogger;

        public SatelliteServiceFactory(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<SentinelHubService> sentinelLogger,
            ILogger<SatelliteImageryService> simulatedLogger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _sentinelLogger = sentinelLogger;
            _simulatedLogger = simulatedLogger;
        }

        public ISatelliteImageryService GetSatelliteService(bool useRealService = false)
        {
            if (useRealService && IsSentinelHubConfigured())
            {
                var httpClient = _httpClientFactory.CreateClient("SentinelHub");
                return new SentinelHubService(httpClient, _configuration, _sentinelLogger);
            }
            
            return new SatelliteImageryService(_simulatedLogger);
        }

        private bool IsSentinelHubConfigured()
        {
            var clientId = _configuration["Satellite:ClientId"];
            var clientSecret = _configuration["Satellite:ClientSecret"];
            var instanceId = _configuration["Satellite:InstanceId"];
            
            return !string.IsNullOrEmpty(clientId) && 
                   !string.IsNullOrEmpty(clientSecret) && 
                   !string.IsNullOrEmpty(instanceId);
        }

    }
}