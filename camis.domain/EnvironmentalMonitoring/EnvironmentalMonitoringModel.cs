using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.EnvironmentalMonitoring
{
    // DTOs for API
    public class ChangeDetectionRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ParcelUpid { get; set; }
        public string? Region { get; set; }
        public decimal ChangeThreshold { get; set; } = 0.15m;
        public List<string>? ChangeTypes { get; set; }
    }

    public class EnvironmentalAnalysisResult
    {
        public List<EnvironmentalChangeEventDto> Changes { get; set; } = new();
        public Dictionary<string, int> ChangeSummary { get; set; } = new();
        public decimal TotalAffectedArea { get; set; }
        public Dictionary<string, decimal> AverageChanges { get; set; } = new();
        public string ChangeMapUrl { get; set; } = string.Empty;
    }

    public class EnvironmentalChangeEventDto
    {
        public string ParcelUpid { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventSubtype { get; set; } = string.Empty;
        public decimal BeforeValue { get; set; }
        public decimal AfterValue { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal ChangePercentage { get; set; }
        public decimal AffectedArea { get; set; }
        public string Severity { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public string Geometry { get; set; } = string.Empty;
        public string Centroid { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class SpectralIndicesRequest
    {
        public DateTime Date { get; set; }
        public string ParcelUpid { get; set; }
        public string GeometryWKT { get; set; }
    }

    // Models/GeoServer/GeoServerRequest.cs
    public class GeoServerRequest
    {
        public string Service { get; set; } = "WMS";
        public string Version { get; set; } = "1.1.1";
        public string Request { get; set; } = "GetMap";
        public string Layers { get; set; } = string.Empty;
        public string Styles { get; set; } = string.Empty;
        public string Format { get; set; } = "image/png";
        public bool Transparent { get; set; } = true;
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public string SRS { get; set; } = "EPSG:20137";
        public string BBOX { get; set; } = string.Empty;
        public string? Time { get; set; }
        public string? CQL_Filter { get; set; }
        public Dictionary<string, string> CustomParameters { get; set; } = new();
    }

// Models/GeoServer/GeoServerLayerInfo.cs
    public class GeoServerLayerInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

// Models/GeoServer/GeoServerWorkspace.cs
    public class GeoServerWorkspace
    {
        public string Name { get; set; } = string.Empty;
    }

// Models/GeoServer/GeoServerStyle.cs
    public class GeoServerStyle
    {
        public string Name { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public string Format { get; set; } = "sld";
        public string StyleContent { get; set; } = string.Empty;
    }

// Models/GeoServer/BoundingBox.cs
    public class BoundingBox
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public override string ToString()
        {
            return $"{MinX},{MinY},{MaxX},{MaxY}";
        }
    }

// Models/GeoServer/WmsLayerRequest.cs
    public class WmsLayerRequest
    {
        public string LayerName { get; set; } = string.Empty;
        public string Bbox { get; set; } = string.Empty;
        public int Width { get; set; } = 512;
        public int Height { get; set; } = 512;
        public string Format { get; set; } = "image/png";
        public string Srs { get; set; } = "EPSG:4326";
        public string Time { get; set; } = string.Empty;
        public string Styles { get; set; } = string.Empty;
        public bool Transparent { get; set; } = true;
    }

// Models/GeoServer/WfsRequest.cs
    public class WfsRequest
    {
        public string TypeName { get; set; } = string.Empty;
        public string OutputFormat { get; set; } = "application/json";
        public string SrsName { get; set; } = "EPSG:20137";
        public string? Bbox { get; set; }
        public string? CqlFilter { get; set; }
        public int MaxFeatures { get; set; } = 1000;
        public string? PropertyName { get; set; }
    }

// Models/GeoServer/GeoServerResponse.cs
    public class GeoServerResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Error { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }

// Models/GeoServer/NDVIAnalysisRequest.cs
    public class NDVIAnalysisRequest
    {
        public string RedBand { get; set; } = "B4";
        public string NirBand { get; set; } = "B8";
        public string Bbox { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string OutputFormat { get; set; } = "image/png";
        public int Width { get; set; } = 512;
        public int Height { get; set; } = 512;
    }

// Models/GeoServer/ChangeDetectionRequest.cs
    public class GeoServerChangeDetectionRequest
    {
        public string BeforeTime { get; set; } = string.Empty;
        public string AfterTime { get; set; } = string.Empty;
        public string Bbox { get; set; } = string.Empty;
        public string LayerName { get; set; } = string.Empty;
        public decimal ChangeThreshold { get; set; } = 0.15m;
        public string OutputFormat { get; set; } = "image/png";
    }
}