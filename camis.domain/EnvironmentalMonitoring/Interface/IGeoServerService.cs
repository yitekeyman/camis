namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface IGeoServerService
{
    // Basic WMS Operations
    Task<string> BuildWMSUrl(GeoServerRequest request);
    Task<Stream?> GetWMSMapAsync(GeoServerRequest request);
    Task<byte[]?> GetWMSMapAsBytesAsync(GeoServerRequest request);
    
    // WFS Operations
    Task<string> ExecuteWFSQueryAsync(WfsRequest request);
    Task<T?> GetWFSFeaturesAsync<T>(WfsRequest request) where T : class;
    
    // Layer Management
    Task<GeoServerResponse<GeoServerLayerInfo>> GetLayerInfoAsync(string workspace, string layerName);
    Task<GeoServerResponse<List<GeoServerLayerInfo>>> GetLayersAsync(string workspace = "");
    Task<GeoServerResponse<bool>> PublishLayerAsync(string workspace, string layerName, string geoJson);
    Task<GeoServerResponse<bool>> DeleteLayerAsync(string workspace, string layerName);
    
    // Workspace Management
    Task<GeoServerResponse<List<GeoServerWorkspace>>> GetWorkspacesAsync();
    Task<GeoServerResponse<bool>> CreateWorkspaceAsync(string workspaceName);
    
    // Utility Methods
    Task<bool> IsGeoServerAvailableAsync();
    Task<string> GetCapabilitiesAsync(string serviceType = "WMS");

    Task<GeoServerResponse<bool>> PublishEnvironmentalChangesAsync(List<EnvironmentalChangeEventDto> changes,
        string workspace = "environment");

    Task<string> BuildEnvironmentalChangesWmsUrl(string bbox, int width = 800, int height = 600);

}