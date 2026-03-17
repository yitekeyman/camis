namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface IGeoServerService
{
    // Basic WMS Operations
    Task<string> BuildWMSUrl(GeoServerRequest request);
    Task<Stream> GetWMSMapAsync(GeoServerRequest request);
    Task<GeoServerResponse<bool>> PublishEnvironmentalChangesAsync(List<EnvironmentalChangeEventDto> changes, string workspace = "environment");
    Task<string> BuildEnvironmentalChangesWmsUrl(string bbox, int width = 800, int height = 600);
    Task<bool> IsGeoServerAvailableAsync();

}