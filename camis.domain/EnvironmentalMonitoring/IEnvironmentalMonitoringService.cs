using intapscamis.camis.data.Entities;

namespace intapscamis.camis.domain.EnvironmentalMonitoring;

public interface IEnvironmentalMonitoringService
{
    Task<EnvironmentalAnalysisResult> AnalyzeEnvironmentalChangesAsync(ChangeDetectionRequest request);
    Task<bool> ProcessSatelliteImageryAsync(DateTime date, string? parcelUpid = null, bool useRealService = false);
    Task<Dictionary<string, decimal>> CalculateSpectralIndicesAsync(string parcelUpid, DateTime date);
    Task<List<ParcelEnvironmentalMonitoring>> GetParcelEnvironmentalHistoryAsync(string parcelUpid, int monthsBack = 12);
    Task<List<EnvironmentalChangeEventDto>> GetSignificantChangesAsync(DateTime startDate, DateTime endDate, string? region = null);
    Task<bool> MonitorFloodRiskAsync(string geometryWkt);
    Task<bool> DetectDroughtConditionsAsync(string region);
    Task<bool> RunScheduledChangeDetectionAsync();
}