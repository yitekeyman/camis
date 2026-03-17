using System.Text;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json;
using System.Globalization;

namespace intapscamis.camis.domain.EnvironmentalMonitoring;

public class GeoServerService : IGeoServerService, IDisposable
{
    private readonly RestClient _client;
    private readonly ILogger<GeoServerService> _logger;
    private readonly string _baseUrl;

    public GeoServerService(
        IConfiguration configuration,
        ILogger<GeoServerService> logger)
    {
        _logger = logger;

        // Get GeoServer configuration
        _baseUrl = configuration["GeoServer:BaseUrl"] ?? "http://localhost:8080";
        var username = configuration["GeoServer:Username"] ?? "admin";
        var password = configuration["GeoServer:Password"] ?? "app-m@ster";

        // Configure RestSharp client
        var options = new RestClientOptions(_baseUrl)
        {
            UserAgent = "camis/1.0"
        };

        _client = new RestClient(options);

        // Manual authentication header
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        _client.AddDefaultHeader("Authorization", $"Basic {authToken}");
    }

    // Basic WMS Operations
    public Task<string> BuildWMSUrl(GeoServerRequest request)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                ["service"] = request.Service,
                ["version"] = request.Version,
                ["request"] = request.Request,
                ["layers"] = request.Layers,
                ["styles"] = request.Styles,
                ["format"] = request.Format,
                ["transparent"] = request.Transparent ? "true" : "false",
                ["width"] = request.Width.ToString(),
                ["height"] = request.Height.ToString(),
                ["srs"] = request.SRS,
                ["bbox"] = request.BBOX
            };

            // Add optional parameters
            if (!string.IsNullOrEmpty(request.Time))
                parameters["time"] = request.Time;

            if (!string.IsNullOrEmpty(request.CQL_Filter))
                parameters["CQL_FILTER"] = request.CQL_Filter;

            // Add custom parameters
            foreach (var customParam in request.CustomParameters)
            {
                parameters[customParam.Key] = customParam.Value;
            }

            var queryString = string.Join("&", parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

            return Task.FromResult($"{_baseUrl}/wms?{queryString}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building WMS URL");
            return Task.FromResult(string.Empty);
        }
    }

    public async Task<Stream> GetWMSMapAsync(GeoServerRequest request)
    {
        try
        {
            var url = await BuildWMSUrl(request);
            if (string.IsNullOrEmpty(url))
                return null;

            var restRequest = new RestRequest(url, Method.Get);
            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful && response.RawBytes != null)
            {
                return new MemoryStream(response.RawBytes);
            }

            _logger.LogError("GeoServer WMS request failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WMS map from GeoServer");
            return null;
        }
    }

    public async Task<byte[]> GetWMSMapAsBytesAsync(GeoServerRequest request)
    {
        try
        {
            var url = await BuildWMSUrl(request);
            if (string.IsNullOrEmpty(url))
                return null;

            var restRequest = new RestRequest(url, Method.Get);
            var response = await _client.ExecuteAsync(restRequest);

            return response.IsSuccessful ? response.RawBytes : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WMS map as bytes");
            return null;
        }
    }

    // WFS Operations
    public async Task<string> ExecuteWFSQueryAsync(WfsRequest request)
    {
        try
        {
            var restRequest = new RestRequest("wfs", Method.Get);

            AddWfsParameters(restRequest, request);

            var response = await _client.ExecuteAsync(restRequest);
            return response.IsSuccessful ? response.Content ?? string.Empty : string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing WFS query");
            return string.Empty;
        }
    }

    public async Task<T> GetWFSFeaturesAsync<T>(WfsRequest request) where T : class
    {
        try
        {
            var jsonResponse = await ExecuteWFSQueryAsync(request);
            return string.IsNullOrEmpty(jsonResponse)
                ? null
                : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WFS features as type {Type}", typeof(T).Name);
            return null;
        }
    }

    // Layer Management
    public async Task<GeoServerResponse<GeoServerLayerInfo>> GetLayerInfoAsync(string workspace, string layerName)
    {
        try
        {
            var restRequest = new RestRequest($"rest/workspaces/{workspace}/layers/{layerName}.json", Method.Get);
            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                var layerInfo =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<GeoServerLayerInfo>(response.Content ?? "{}");
                return CreateSuccessResponse(layerInfo, response.StatusCode);
            }

            return CreateErrorResponse<GeoServerLayerInfo>($"Failed to get layer info: {response.StatusCode}",
                response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting layer info for {Workspace}:{LayerName}", workspace, layerName);
            return CreateErrorResponse<GeoServerLayerInfo>(ex.Message);
        }
    }

    public async Task<GeoServerResponse<List<GeoServerLayerInfo>>> GetLayersAsync(string workspace = "")
    {
        try
        {
            var url = string.IsNullOrEmpty(workspace)
                ? "rest/layers.json"
                : $"rest/workspaces/{workspace}/layers.json";

            var restRequest = new RestRequest(url, Method.Get);
            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                var layers = ParseLayersFromJson(response.Content ?? "{}");
                return CreateSuccessResponse(layers, response.StatusCode);
            }

            return CreateErrorResponse<List<GeoServerLayerInfo>>($"Failed to get layers: {response.StatusCode}",
                response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting layers from GeoServer");
            return CreateErrorResponse<List<GeoServerLayerInfo>>(ex.Message);
        }
    }

    public async Task<GeoServerResponse<bool>> PublishLayerAsync(string workspace, string layerName, string geoJson)
    {
        try
        {
            // First, create the datastore if it doesn't exist
            var datastoreName = $"{layerName}_store";
            var createStoreResult = await CreateDataStoreAsync(workspace, datastoreName);

            if (!createStoreResult.Success)
                return createStoreResult;

            // Upload GeoJSON to the datastore
            var uploadUrl = $"rest/workspaces/{workspace}/datastores/{datastoreName}/file.geojson";
            var restRequest = new RestRequest(uploadUrl, Method.Put);
            restRequest.AddStringBody(geoJson, ContentType.Json);

            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Successfully published layer {LayerName} in workspace {Workspace}", layerName,
                    workspace);
                return CreateSuccessResponse(true, response.StatusCode);
            }

            _logger.LogError("Failed to publish layer. Status: {StatusCode}, Error: {Error}", response.StatusCode,
                response.Content);
            return CreateErrorResponse<bool>($"Failed to publish layer: {response.StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing layer {LayerName} to GeoServer", layerName);
            return CreateErrorResponse<bool>(ex.Message);
        }
    }

    public async Task<GeoServerResponse<bool>> DeleteLayerAsync(string workspace, string layerName)
    {
        try
        {
            var restRequest = new RestRequest($"rest/workspaces/{workspace}/layers/{layerName}", Method.Delete);
            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Successfully deleted layer {LayerName} from workspace {Workspace}", layerName,
                    workspace);
                return CreateSuccessResponse(true, response.StatusCode);
            }

            return CreateErrorResponse<bool>($"Failed to delete layer: {response.StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting layer {LayerName} from GeoServer", layerName);
            return CreateErrorResponse<bool>(ex.Message);
        }
    }

    // Workspace Management
    public async Task<GeoServerResponse<List<GeoServerWorkspace>>> GetWorkspacesAsync()
    {
        try
        {
            var restRequest = new RestRequest("rest/workspaces.json", Method.Get);
            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                var workspaces = ParseWorkspacesFromJson(response.Content ?? "{}");
                return CreateSuccessResponse(workspaces, response.StatusCode);
            }

            return CreateErrorResponse<List<GeoServerWorkspace>>($"Failed to get workspaces: {response.StatusCode}",
                response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workspaces from GeoServer");
            return CreateErrorResponse<List<GeoServerWorkspace>>(ex.Message);
        }
    }

    public async Task<GeoServerResponse<bool>> CreateWorkspaceAsync(string workspaceName)
    {
        try
        {
            var restRequest = new RestRequest("rest/workspaces", Method.Post);
            var workspaceJson = $"{{\"workspace\": {{\"name\": \"{workspaceName}\"}}}}";
            restRequest.AddStringBody(workspaceJson, ContentType.Json);

            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Successfully created workspace {WorkspaceName}", workspaceName);
                return CreateSuccessResponse(true, response.StatusCode);
            }

            return CreateErrorResponse<bool>($"Failed to create workspace: {response.StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workspace {WorkspaceName}", workspaceName);
            return CreateErrorResponse<bool>(ex.Message);
        }
    }

    // Utility Methods
    public async Task<bool> IsGeoServerAvailableAsync()
    {
        try
        {
            var restRequest = new RestRequest("rest/about/version", Method.Get);
            var response = await _client.ExecuteAsync(restRequest);
            return response.IsSuccessful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeoServer is not available");
            return false;
        }
    }

    public async Task<string> GetCapabilitiesAsync(string serviceType = "WMS")
    {
        try
        {
            var restRequest = new RestRequest($"{serviceType.ToLower()}", Method.Get);
            restRequest.AddParameter("service", serviceType);
            restRequest.AddParameter("version", "1.1.1");
            restRequest.AddParameter("request", "GetCapabilities");

            var response = await _client.ExecuteAsync(restRequest);
            return response.IsSuccessful ? response.Content ?? string.Empty : string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capabilities from GeoServer");
            return string.Empty;
        }
    }

    public async Task<GeoServerResponse<bool>> PublishEnvironmentalChangesAsync(
        List<EnvironmentalChangeEventDto> changes, string workspace = "environment")
    {
        try
        {
            // Create workspace if it doesn't exist
            var workspaceResult = await CreateWorkspaceAsync(workspace);
            if (!workspaceResult.Success && workspaceResult.StatusCode != 409) // 409 = already exists
            {
                return CreateErrorResponse<bool>($"Failed to create workspace: {workspaceResult.Error}");
            }

            // Convert changes to GeoJSON
            var geoJson = ConvertChangesToSimpleGeoJson(changes);

            // Publish layer
            var layerName = "environmental_changes";
            var result = await PublishLayerAsync(workspace, layerName, geoJson);

            if (result.Success)
            {
                _logger.LogInformation("Successfully published environmental changes layer with {Count} features",
                    changes.Count);

                // Create and apply style
                await CreateEnvironmentalChangeStyleAsync(workspace, layerName);
                return CreateSuccessResponse(true, HttpStatusCode.OK);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing environmental changes layer");
            return CreateErrorResponse<bool>(ex.Message);
        }
    }

    private double[][][] ParseWktToCoordinates(string wkt)
    {
        var coordinates = new List<double[]>();

        try
        {
            // First try using NetTopologySuite's WKT reader
            var reader = new NetTopologySuite.IO.WKTReader();
            var geometry = reader.Read(wkt);

            if (geometry != null)
            {
                // Extract coordinates from the geometry
                foreach (var coord in geometry.Coordinates)
                {
                    coordinates.Add(new[] { coord.X, coord.Y });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse WKT coordinates with NetTopologySuite: {Wkt}", wkt);

            // Fallback: manual parsing for common WKT formats
            try
            {
                // Handle POLYGON ((x y, x y, ...))
                if (wkt.StartsWith("POLYGON", StringComparison.OrdinalIgnoreCase))
                {
                    var startIndex = wkt.IndexOf("((");
                    var endIndex = wkt.LastIndexOf("))");

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        startIndex += 2; // Move past "(("
                        var coordString = wkt.Substring(startIndex, endIndex - startIndex);
                        var coordPairs = coordString.Split(',');

                        foreach (var coordPair in coordPairs)
                        {
                            var parts = coordPair.Trim()
                                .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 2)
                            {
                                // Use InvariantCulture to handle decimal points correctly
                                if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture,
                                        out double x) &&
                                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture,
                                        out double y))
                                {
                                    coordinates.Add(new[] { x, y });
                                }
                            }
                        }
                    }
                }
                // Handle POINT (x y)
                else if (wkt.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
                {
                    var startIndex = wkt.IndexOf('(');
                    var endIndex = wkt.IndexOf(')');

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        startIndex += 1; // Move past "("
                        var coordString = wkt.Substring(startIndex, endIndex - startIndex);
                        var parts = coordString.Trim()
                            .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 2)
                        {
                            if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture,
                                    out double x) &&
                                double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                            {
                                coordinates.Add(new[] { x, y });
                            }
                        }
                    }
                }
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Fallback WKT parsing also failed for: {Wkt}", wkt);
            }
        }

        // Ensure we have at least one coordinate
        if (coordinates.Count == 0)
        {
            coordinates.Add(new[] { 0.0, 0.0 });
        }

        return new[] { coordinates.ToArray() };
    }

    private string ConvertChangesToSimpleGeoJson(List<EnvironmentalChangeEventDto> changes)
    {
        var features = new List<object>();

        foreach (var change in changes)
        {
            // Use centroid for point geometry (simpler than parsing polygons)
            var (longitude, latitude) = ParseCentroidCoordinates(change.Centroid);

            var feature = new
            {
                type = "Feature",
                geometry = new
                {
                    type = "Point",
                    coordinates = new[] { longitude, latitude }
                },
                properties = new
                {
                    id = change.ParcelUpid,
                    eventType = change.EventType,
                    eventSubtype = change.EventSubtype,
                    severity = change.Severity,
                    confidence = change.Confidence,
                    changeAmount = change.ChangeAmount,
                    changePercentage = change.ChangePercentage,
                    affectedArea = change.AffectedArea,
                    description = change.Description,
                    eventDate = change.EventDate.ToString("yyyy-MM-dd"),
                    hasDetailedGeometry = !string.IsNullOrEmpty(change.Geometry)
                }
            };
            features.Add(feature);
        }

        var featureCollection = new
        {
            type = "FeatureCollection",
            features = features
        };

        return JsonConvert.SerializeObject(featureCollection);
    }

    private (double longitude, double latitude) ParseCentroidCoordinates(string centroidWkt)
    {
        if (string.IsNullOrEmpty(centroidWkt))
            return (0, 0);

        try
        {
            // Simple parsing for POINT(x y) format
            if (centroidWkt.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
            {
                var startIndex = centroidWkt.IndexOf('(');
                var endIndex = centroidWkt.IndexOf(')');

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    var coordString = centroidWkt.Substring(startIndex + 1, endIndex - startIndex - 1);
                    var parts = coordString.Trim().Split(' ');

                    if (parts.Length >= 2)
                    {
                        // Use invariant culture for consistent parsing
                        if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                            double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                        {
                            return (x, y);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse centroid coordinates: {Centroid}", centroidWkt);
        }

        return (0, 0);
    }

    private (double longitude, double latitude) ParseCentroidCoordinatesSimple(string centroidWkt)
    {
        if (string.IsNullOrEmpty(centroidWkt))
            return (0, 0);

        try
        {
            // Simple parsing that doesn't rely on CultureInfo
            if (centroidWkt.StartsWith("POINT", StringComparison.OrdinalIgnoreCase))
            {
                var startIndex = centroidWkt.IndexOf('(');
                var endIndex = centroidWkt.IndexOf(')');

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    var coordString = centroidWkt.Substring(startIndex + 1, endIndex - startIndex - 1);
                    var parts = coordString.Trim().Split(' ');

                    if (parts.Length >= 2)
                    {
                        // Simple parsing without CultureInfo
                        if (double.TryParse(parts[0], out double x) &&
                            double.TryParse(parts[1], out double y))
                        {
                            return (x, y);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse centroid coordinates: {Centroid}", centroidWkt);
        }

        return (0, 0);
    }

    private async Task<bool> CreateEnvironmentalChangeStyleAsync(string workspace, string layerName)
    {
        try
        {
            var sld = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<StyledLayerDescriptor version=""1.0.0"" 
    xsi:schemaLocation=""http://www.opengis.net/sld StyledLayerDescriptor.xsd"" 
    xmlns=""http://www.opengis.net/sld"" 
    xmlns:ogc=""http://www.opengis.net/ogc"" 
    xmlns:xlink=""http://www.w3.org/1999/xlink"" 
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <NamedLayer>
    <Name>" + layerName + @"</Name>
    <UserStyle>
      <Title>Environmental Changes</Title>
      <FeatureTypeStyle>
        <Rule>
          <Title>Vegetation Loss</Title>
          <ogc:Filter>
            <ogc:PropertyIsEqualTo>
              <ogc:PropertyName>eventType</ogc:PropertyName>
              <ogc:Literal>VEGETATION_LOSS</ogc:Literal>
            </ogc:PropertyIsEqualTo>
          </ogc:Filter>
          <PolygonSymbolizer>
            <Fill>
              <CssParameter name=""fill"">#FF0000</CssParameter>
              <CssParameter name=""fill-opacity"">0.4</CssParameter>
            </Fill>
            <Stroke>
              <CssParameter name=""stroke"">#FF0000</CssParameter>
              <CssParameter name=""stroke-width"">2</CssParameter>
            </Stroke>
          </PolygonSymbolizer>
        </Rule>
        <Rule>
          <Title>Vegetation Growth</Title>
          <ogc:Filter>
            <ogc:PropertyIsEqualTo>
              <ogc:PropertyName>eventType</ogc:PropertyName>
              <ogc:Literal>VEGETATION_GROWTH</ogc:Literal>
            </ogc:PropertyIsEqualTo>
          </ogc:Filter>
          <PolygonSymbolizer>
            <Fill>
              <CssParameter name=""fill"">#00FF00</CssParameter>
              <CssParameter name=""fill-opacity"">0.4</CssParameter>
            </Fill>
            <Stroke>
              <CssParameter name=""stroke"">#00FF00</CssParameter>
              <CssParameter name=""stroke-width"">2</CssParameter>
            </Stroke>
          </PolygonSymbolizer>
        </Rule>
        <Rule>
          <Title>Water Increase</Title>
          <ogc:Filter>
            <ogc:PropertyIsEqualTo>
              <ogc:PropertyName>eventType</ogc:PropertyName>
              <ogc:Literal>WATER_INCREASE</ogc:Literal>
            </ogc:PropertyIsEqualTo>
          </ogc:Filter>
          <PolygonSymbolizer>
            <Fill>
              <CssParameter name=""fill"">#0000FF</CssParameter>
              <CssParameter name=""fill-opacity"">0.4</CssParameter>
            </Fill>
            <Stroke>
              <CssParameter name=""stroke"">#0000FF</CssParameter>
              <CssParameter name=""stroke-width"">2</CssParameter>
            </Stroke>
          </PolygonSymbolizer>
        </Rule>
        <Rule>
          <Title>Water Decrease</Title>
          <ogc:Filter>
            <ogc:PropertyIsEqualTo>
              <ogc:PropertyName>eventType</ogc:PropertyName>
              <ogc:Literal>WATER_DECREASE</ogc:Literal>
            </ogc:PropertyIsEqualTo>
          </ogc:Filter>
          <PolygonSymbolizer>
            <Fill>
              <CssParameter name=""fill"">#FFA500</CssParameter>
              <CssParameter name=""fill-opacity"">0.4</CssParameter>
            </Fill>
            <Stroke>
              <CssParameter name=""stroke"">#FFA500</CssParameter>
              <CssParameter name=""stroke-width"">2</CssParameter>
            </Stroke>
          </PolygonSymbolizer>
        </Rule>
        <Rule>
          <Title>Urbanization</Title>
          <ogc:Filter>
            <ogc:PropertyIsEqualTo>
              <ogc:PropertyName>eventType</ogc:PropertyName>
              <ogc:Literal>URBANIZATION</ogc:Literal>
            </ogc:PropertyIsEqualTo>
          </ogc:Filter>
          <PolygonSymbolizer>
            <Fill>
              <CssParameter name=""fill"">#808080</CssParameter>
              <CssParameter name=""fill-opacity"">0.4</CssParameter>
            </Fill>
            <Stroke>
              <CssParameter name=""stroke"">#808080</CssParameter>
              <CssParameter name=""stroke-width"">2</CssParameter>
            </Stroke>
          </PolygonSymbolizer>
        </Rule>
      </FeatureTypeStyle>
    </UserStyle>
  </NamedLayer>
</StyledLayerDescriptor>";

            var restRequest = new RestRequest($"rest/workspaces/{workspace}/styles/{layerName}", Method.Put);
            restRequest.AddHeader("Content-Type", "application/vnd.ogc.sld+xml");
            restRequest.AddStringBody(sld, "application/vnd.ogc.sld+xml");

            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                // Apply the style to the layer
                var applyStyleRequest = new RestRequest($"rest/workspaces/{workspace}/layers/{layerName}", Method.Put);
                var styleJson = $"{{\"layer\": {{\"defaultStyle\": {{\"name\": \"{layerName}\"}}}}}}";
                applyStyleRequest.AddStringBody(styleJson, ContentType.Json);

                var applyResponse = await _client.ExecuteAsync(applyStyleRequest);
                return applyResponse.IsSuccessful;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating environmental change style");
            return false;
        }
    }

    public async Task<string> BuildEnvironmentalChangesWmsUrl(string bbox, int width = 800, int height = 600)
    {
        var request = new GeoServerRequest
        {
            Layers = "environment:environmental_changes",
            Styles = "environmental_changes",
            BBOX = bbox,
            Width = width,
            Height = height,
            SRS = "EPSG:20137"
        };

        return await BuildWMSUrl(request);
    }

    // Helper Methods
    private async Task<GeoServerResponse<bool>> CreateDataStoreAsync(string workspace, string datastoreName)
    {
        try
        {
            var restRequest = new RestRequest($"rest/workspaces/{workspace}/datastores", Method.Post);
            var datastoreJson = $"{{\"dataStore\": {{\"name\": \"{datastoreName}\", \"type\": \"GeoJSON\"}}}}";
            restRequest.AddStringBody(datastoreJson, ContentType.Json);

            var response = await _client.ExecuteAsync(restRequest);

            if (response.IsSuccessful || response.StatusCode == HttpStatusCode.Conflict)
            {
                return CreateSuccessResponse(true, response.StatusCode);
            }

            return CreateErrorResponse<bool>($"Failed to create datastore: {response.StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating datastore {DatastoreName}", datastoreName);
            return CreateErrorResponse<bool>(ex.Message);
        }
    }

    private void AddWfsParameters(RestRequest request, WfsRequest wfsRequest)
    {
        request.AddParameter("service", "WFS");
        request.AddParameter("version", "1.1.0");
        request.AddParameter("request", "GetFeature");
        request.AddParameter("typeName", wfsRequest.TypeName);
        request.AddParameter("outputFormat", wfsRequest.OutputFormat);
        request.AddParameter("srsName", wfsRequest.SrsName);

        if (!string.IsNullOrEmpty(wfsRequest.Bbox))
            request.AddParameter("bbox", wfsRequest.Bbox);

        if (!string.IsNullOrEmpty(wfsRequest.CqlFilter))
            request.AddParameter("CQL_FILTER", wfsRequest.CqlFilter);

        if (wfsRequest.MaxFeatures > 0)
            request.AddParameter("maxFeatures", wfsRequest.MaxFeatures.ToString());

        if (!string.IsNullOrEmpty(wfsRequest.PropertyName))
            request.AddParameter("propertyName", wfsRequest.PropertyName);
    }

    private GeoServerResponse<T> CreateSuccessResponse<T>(T data, HttpStatusCode statusCode)
    {
        return new GeoServerResponse<T>
        {
            Success = true,
            Data = data,
            StatusCode = (int)statusCode
        };
    }

    private GeoServerResponse<T> CreateErrorResponse<T>(string error,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new GeoServerResponse<T>
        {
            Success = false,
            Error = error,
            StatusCode = (int)statusCode
        };
    }

    // JSON parsing helper methods
    private List<GeoServerLayerInfo> ParseLayersFromJson(string json)
    {
        try
        {
            var layers = new List<GeoServerLayerInfo>();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("layers", out var layersElement) &&
                layersElement.TryGetProperty("layer", out var layerArray))
            {
                foreach (var layerElement in layerArray.EnumerateArray())
                {
                    var layer = new GeoServerLayerInfo
                    {
                        Name = layerElement.GetProperty("name").GetString() ?? string.Empty,
                        Title = layerElement.GetProperty("title").GetString() ?? string.Empty
                    };
                    layers.Add(layer);
                }
            }

            return layers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing layers from JSON");
            return new List<GeoServerLayerInfo>();
        }
    }

    private List<GeoServerWorkspace> ParseWorkspacesFromJson(string json)
    {
        try
        {
            var workspaces = new List<GeoServerWorkspace>();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("workspaces", out var workspacesElement) &&
                workspacesElement.TryGetProperty("workspace", out var workspaceArray))
            {
                foreach (var workspaceElement in workspaceArray.EnumerateArray())
                {
                    var workspace = new GeoServerWorkspace
                    {
                        Name = workspaceElement.GetProperty("name").GetString() ?? string.Empty
                    };
                    workspaces.Add(workspace);
                }
            }

            return workspaces;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing workspaces from JSON");
            return new List<GeoServerWorkspace>();
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}