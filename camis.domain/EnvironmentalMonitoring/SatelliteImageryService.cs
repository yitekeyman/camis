using System.Net.Http.Json;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.EnvironmentalMonitoring;

public class SatelliteImageryService: ISatelliteImageryService
{
    private readonly ILogger<SatelliteImageryService> _logger;
    //private readonly IConfiguration _configuration;
    
    public SatelliteImageryService(ILogger<SatelliteImageryService> logger)
    {
        _logger = logger;
       // _configuration = configuration;
    }
    
    public async Task<SatelliteImage> GetImageryAsync(Geometry geometry, DateTime date)
    {
        try
        {
            // For testing without real API calls
            // This simulates realistic spectral values based on land cover types
            var random = new Random(geometry.Centroid.X.GetHashCode() + date.GetHashCode());
            
            // Simulate different land cover types
            var landCoverType = random.Next(0, 4); // 0: Forest, 1: Agriculture, 2: Urban, 3: Water
            
            var (red, green, nir, swir) = landCoverType switch
            {
                0 => (0.05m, 0.08m, 0.4m, 0.1m),    // Forest - high NIR
                1 => (0.1m, 0.15m, 0.3m, 0.08m),    // Agriculture - medium NIR
                2 => (0.15m, 0.12m, 0.15m, 0.2m),   // Urban - high SWIR
                3 => (0.02m, 0.03m, 0.01m, 0.01m),  // Water - low all
                _ => (0.1m, 0.1m, 0.1m, 0.1m)
            };

            // Add seasonal variation
            var seasonalFactor = 0.1m * (decimal)Math.Sin((date.DayOfYear / 365.0) * 2 * Math.PI);
            nir += seasonalFactor;
            red += seasonalFactor * 0.5m;

            var image = new SatelliteImage
            {
                Data = Array.Empty<byte>(), // No actual image data for testing
                Date = date,
                Bounds = geometry.EnvelopeInternal.ToString(),
                Bands = new[] { "B02", "B03", "B04", "B08", "B11", "B12" }
            };

            // Store reflectance values
            image.ReflectanceValues["B04"] = red;    // Red band
            image.ReflectanceValues["B03"] = green;  // Green band  
            image.ReflectanceValues["B08"] = nir;    // NIR band
            image.ReflectanceValues["B11"] = swir;   // SWIR band

            return image;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SimpleSatelliteService.GetImageryAsync");
            return null;
        }
    }

    public async Task<Dictionary<string, decimal>> CalculateSpectralIndicesAsync(SatelliteImage image)
    {
        if (image?.ReflectanceValues == null || !image.ReflectanceValues.Any()) 
        {
            _logger.LogWarning("No reflectance values available for spectral index calculation");
            return new Dictionary<string, decimal>();
        }

        try
        {
            // Get reflectance values with safe defaults
            var red = image.ReflectanceValues.GetValueOrDefault("B04", 0.1m);
            var green = image.ReflectanceValues.GetValueOrDefault("B03", 0.1m);
            var nir = image.ReflectanceValues.GetValueOrDefault("B08", 0.1m);
            var swir = image.ReflectanceValues.GetValueOrDefault("B11", 0.1m);

            var indices = new Dictionary<string, decimal>
            {
                ["NDVI"] = CalculateNDVI(nir, red),
                ["NDWI"] = CalculateNDWI(green, nir),
                ["MNDWI"] = CalculateMNDWI(green, swir),
                ["NDBI"] = CalculateNDBI(swir, nir),
                ["EVI"] = CalculateEVI(nir, red, image.ReflectanceValues.GetValueOrDefault("B02", 0.1m))
            };

            _logger.LogDebug("Calculated spectral indices: NDVI={NDVI}, NDWI={NDWI}", indices["NDVI"], indices["NDWI"]);
            return indices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating spectral indices");
            return new Dictionary<string, decimal>();
        }
    }

    private decimal CalculateNDVI(decimal nir, decimal red)
    {
        if (nir + red == 0) return 0;
        return (nir - red) / (nir + red);
    }

    private decimal CalculateNDWI(decimal green, decimal nir)
    {
        if (green + nir == 0) return 0;
        return (green - nir) / (green + nir);
    }

    private decimal CalculateMNDWI(decimal green, decimal swir)
    {
        if (green + swir == 0) return 0;
        return (green - swir) / (green + swir);
    }

    private decimal CalculateNDBI(decimal swir, decimal nir)
    {
        if (swir + nir == 0) return 0;
        return (swir - nir) / (swir + nir);
    }

    private decimal CalculateEVI(decimal nir, decimal red, decimal blue)
    {
        if (nir + 6m * red - 7.5m * blue + 1m == 0) return 0;
        return 2.5m * (nir - red) / (nir + 6m * red - 7.5m * blue + 1m);
    }

    public async Task<List<DateTime>> GetAvailableDatesAsync(Geometry geometry, DateTime start, DateTime end)
    {
        var dates = new List<DateTime>();
        var current = start;
        
        // Generate dates with 5-day intervals (simulating Sentinel-2 revisit cycle)
        while (current <= end)
        {
            if ((current - start).Days % 5 == 0) // Every 5 days
            {
                dates.Add(current);
            }
            current = current.AddDays(1);
        }
        
        return dates;
    }

    public async Task<bool> IsImageCloudFreeAsync(SatelliteImage image)
    {
        // Simple cloud detection simulation
        // In reality, you'd analyze cloud probability
        var random = new Random(image.Date.GetHashCode());
        return random.NextDouble() > 0.3; // 70% chance of cloud-free
    }
}