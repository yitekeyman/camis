using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace intapscamis.camis.domain.EnvironmentalMonitoring;

// Replace your existing EnvironmentalMonitoringService with this complete version

public class EnvironmentalMonitoringService : IEnvironmentalMonitoringService
{
    private readonly IEnvironmentalMonitoringRepository _monitoringRepo;
    private readonly IEnvironmentalChangeEventRepository _changeEventRepo;
    private readonly IParcelViewRepository _parcelViewRepo;
    private readonly IGeoServerService _geoServer;
    private readonly ILogger<EnvironmentalMonitoringService> _logger;
    private readonly CamisContext _context;

    public EnvironmentalMonitoringService(
        IEnvironmentalMonitoringRepository monitoringRepo,
        IEnvironmentalChangeEventRepository changeEventRepo,
        IParcelViewRepository parcelViewRepo,
        IGeoServerService geoserver,
        CamisContext context,
        ILogger<EnvironmentalMonitoringService> logger)
    {
        _monitoringRepo = monitoringRepo;
        _changeEventRepo = changeEventRepo;
        _parcelViewRepo = parcelViewRepo;
        _geoServer = geoserver;
        _logger = logger;
        _context = context;
    }

    public async Task<EnvironmentalAnalysisResult> AnalyzeEnvironmentalChangesAsync(ChangeDetectionRequest request)
    {
        _logger.LogInformation("Starting environmental change analysis for {StartDate} to {EndDate}", 
            request.StartDate, request.EndDate);

        var result = new EnvironmentalAnalysisResult();

        try
        {
            var changes = await DetectChangesUsingEntityFrameworkAsync(request);
            
            result.Changes = changes;
            result.ChangeSummary = GenerateChangeSummary(changes);
            result.TotalAffectedArea = changes.Sum(c => c.AffectedArea);
            result.AverageChanges = CalculateAverageChanges(changes);
            
            if (changes.Any())
            {
                // Publish changes to GeoServer
                var publishResult = await _geoServer.PublishEnvironmentalChangesAsync(changes);
                if (publishResult.Success)
                {
                    var bbox = await _parcelViewRepo.GetRegionBoundingBoxAsync(request.Region);
                    result.ChangeMapUrl = await _geoServer.BuildEnvironmentalChangesWmsUrl(bbox);
                    _logger.LogInformation("Environmental changes published to GeoServer successfully");
                }
            }

            await StoreChangeEventsAsync(changes);

            _logger.LogInformation("Environmental analysis completed. Found {Count} changes", changes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in environmental change analysis");
            throw;
        }

        return result;
    }

    public async Task<bool> ProcessSatelliteImageryAsync(DateTime date, string? parcelUpid = null)
    {
        try
        {
            var parcels = await GetParcelsForProcessingAsync(parcelUpid);
            _logger.LogInformation("Processing satellite imagery for {Count} parcels on {Date}", parcels.Count, date);

            foreach (var parcel in parcels)
            {
                var spectralData = await CalculateSpectralIndicesForParcelAsync(parcel, date);
                if (spectralData != null)
                {
                    await StoreEnvironmentalMonitoringDataAsync(parcel, date, spectralData);
                }
            }

            await _monitoringRepo.SaveAsync();
            _logger.LogInformation("Satellite imagery processing completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing satellite imagery");
            return false;
        }
    }

    public async Task<Dictionary<string, decimal>> CalculateSpectralIndicesAsync(string parcelUpid, DateTime date)
    {
        var monitoringData = await _monitoringRepo.GetByParcelAndDateRangeAsync(parcelUpid, date, date);
        var data = monitoringData.FirstOrDefault();

        if (data == null)
        {
            _logger.LogWarning("No monitoring data found for parcel {ParcelUpid} on {Date}", parcelUpid, date);
            return new Dictionary<string, decimal>();
        }

        return CreateSpectralIndicesDictionary(data);
    }

    public async Task<List<ParcelEnvironmentalMonitoring>> GetParcelEnvironmentalHistoryAsync(string parcelUpid, int monthsBack = 12)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-monthsBack);
        return (await _monitoringRepo.GetByParcelAndDateRangeAsync(parcelUpid, cutoffDate, DateTime.UtcNow))
            .ToList();
    }

    public async Task<List<EnvironmentalChangeEventDto>> GetSignificantChangesAsync(DateTime startDate, DateTime endDate, string? region = null)
    {
        var changeEvents = await _changeEventRepo.GetSignificantEventsAsync(startDate, endDate);
        
        return changeEvents.Select(e => MapToChangeEventDto(e)).ToList();
    }

    public async Task<bool> MonitorFloodRiskAsync(string geometryWkt)
    {
        try
        {
            var geometry = CreateGeometryFromWkt(geometryWkt);
            var parcels = await _parcelViewRepo.GetWithinGeometryAsync(geometry);
            
            var parcelUpids = parcels.Select(p => p.Upid).ToList();
            var recentData = await _monitoringRepo.GetByDateAsync(DateTime.UtcNow.AddDays(-7));

            var waterIndices = recentData
                .Where(d => parcelUpids.Contains(d.ParcelUpid) && d.NDWI.HasValue && d.MNDWI.HasValue)
                .ToList();

            var avgNdwi = waterIndices.Average(d => d.NDWI) ?? 0;
            var avgMndwi = waterIndices.Average(d => d.MNDWI) ?? 0;

            bool floodRisk = avgNdwi > 0.3m || avgMndwi > 0.2m;
            
            _logger.LogInformation("Flood risk assessment: {Risk} (NDWI: {NDWI}, MNDWI: {MNDWI})", 
                floodRisk ? "HIGH" : "LOW", avgNdwi, avgMndwi);
                
            return floodRisk;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error monitoring flood risk");
            return false;
        }
    }

    public async Task<bool> DetectDroughtConditionsAsync(string region)
    {
        try
        {
            var parcels = await _parcelViewRepo.GetByRegionAsync(region);
            var parcelUpids = parcels.Select(p => p.Upid).ToList();
            
            var recentData = await _monitoringRepo.GetByDateAsync(DateTime.UtcNow.AddDays(-30));
            var regionData = recentData.Where(d => parcelUpids.Contains(d.ParcelUpid)).ToList();

            var avgNdvi = regionData.Average(d => d.NDVI) ?? 0;
            var avgNdwi = regionData.Average(d => d.NDWI) ?? 0;
            var avgMoisture = regionData.Average(d => d.SoilMoisture) ?? 0;

            bool droughtConditions = avgNdvi < 0.3m && avgNdwi < 0.1m && avgMoisture < 0.2m;
            
            _logger.LogInformation("Drought conditions assessment for {Region}: {Conditions} (NDVI: {NDVI}, NDWI: {NDWI}, Moisture: {Moisture})", 
                region, droughtConditions ? "DROUGHT" : "NORMAL", avgNdvi, avgNdwi, avgMoisture);
                
            return droughtConditions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting drought conditions");
            return false;
        }
    }

    // Private helper methods
    private async Task<List<EnvironmentalChangeEventDto>> DetectChangesUsingEntityFrameworkAsync(ChangeDetectionRequest request)
    {
        var changes = new List<EnvironmentalChangeEventDto>();
        var parcels = await GetParcelsForAnalysisAsync(request.ParcelUpid, request.Region);
        
        _logger.LogInformation("Analyzing changes for {Count} parcels", parcels.Count);

        foreach (var parcel in parcels)
        {
            var parcelChanges = await AnalyzeParcelChangesAsync(parcel, request.StartDate, request.EndDate, request.ChangeThreshold);
            changes.AddRange(parcelChanges);
        }

        // Filter by change types if specified
        if (request.ChangeTypes?.Any() == true)
        {
            changes = changes.Where(c => request.ChangeTypes.Contains(c.EventType)).ToList();
        }

        return changes;
    }

    private async Task<List<EnvironmentalChangeEventDto>> AnalyzeParcelChangesAsync(string parcelUpid, DateTime startDate, DateTime endDate, decimal threshold)
    {
        var monitoringData = await _monitoringRepo.GetByParcelAndDateRangeAsync(parcelUpid, startDate, endDate);
        var startData = monitoringData.OrderBy(m => m.MonitoringDate).FirstOrDefault();
        var endData = monitoringData.OrderByDescending(m => m.MonitoringDate).FirstOrDefault();

        if (startData == null || endData == null)
        {
            _logger.LogDebug("Insufficient data for parcel {ParcelUpid}", parcelUpid);
            return new List<EnvironmentalChangeEventDto>();
        }

        // Calculate changes
        var ndviChange = (endData.NDVI ?? 0) - (startData.NDVI ?? 0);
        var ndwiChange = (endData.NDWI ?? 0) - (startData.NDWI ?? 0);
        var ndbiChange = (endData.NDBI ?? 0) - (startData.NDBI ?? 0);

        // Check if changes exceed threshold
        if (Math.Abs(ndviChange) >= threshold || Math.Abs(ndwiChange) >= threshold || Math.Abs(ndbiChange) >= threshold)
        {
            var changeEvent = await CreateChangeEventAsync(parcelUpid, startData, endData, ndviChange, ndwiChange, ndbiChange, threshold);
            return changeEvent != null ? new List<EnvironmentalChangeEventDto> { changeEvent } : new List<EnvironmentalChangeEventDto>();
        }

        return new List<EnvironmentalChangeEventDto>();
    }

    private async Task<EnvironmentalChangeEventDto?> CreateChangeEventAsync(
        string parcelUpid, 
        ParcelEnvironmentalMonitoring startData, 
        ParcelEnvironmentalMonitoring endData,
        decimal ndviChange, 
        decimal ndwiChange, 
        decimal ndbiChange,
        decimal threshold)
    {
        var (eventType, eventSubtype, severity) = ClassifyChange(ndviChange, ndwiChange, ndbiChange, threshold);
        
        if (eventType == "NO_CHANGE")
            return null;

        var geometry = await _parcelViewRepo.GetParcelGeometryAsync(parcelUpid);
        var area = await CalculateParcelAreaAsync(parcelUpid);

        return new EnvironmentalChangeEventDto
        {
            ParcelUpid = parcelUpid,
            EventDate = endData.MonitoringDate,
            EventType = eventType,
            EventSubtype = eventSubtype,
            BeforeValue = startData.NDVI ?? 0,
            AfterValue = endData.NDVI ?? 0,
            ChangeAmount = ndviChange,
            ChangePercentage = startData.NDVI.HasValue && startData.NDVI.Value != 0 ? 
                (ndviChange / startData.NDVI.Value) * 100 : 0,
            AffectedArea = area,
            Severity = severity,
            Confidence = CalculateConfidence(ndviChange, ndwiChange),
            Geometry = geometry?.AsText() ?? string.Empty,
            Centroid = geometry?.Centroid?.AsText() ?? string.Empty,
            Description = GenerateChangeDescription(eventType, eventSubtype, ndviChange, ndwiChange)
        };
    }

    private (string EventType, string EventSubtype, string Severity) ClassifyChange(decimal ndviChange, decimal ndwiChange, decimal ndbiChange, decimal threshold)
    {
        string eventType = "NO_CHANGE";
        string eventSubtype = string.Empty;
        string severity = "LOW";

        // Determine severity based on change magnitude
        var maxChange = Math.Max(Math.Abs(ndviChange), Math.Abs(ndwiChange));
        if (maxChange > 0.4m)
            severity = "SEVERE";
        else if (maxChange > 0.25m)
            severity = "HIGH";
        else if (maxChange > 0.15m)
            severity = "MODERATE";

        // Classify change type based on spectral indices
        if (ndviChange < -threshold)
        {
            eventType = "VEGETATION_LOSS";
            eventSubtype = ndviChange < -0.3m ? "SEVERE_DEGRADATION" : "MODERATE_LOSS";
        }
        else if (ndviChange > threshold)
        {
            eventType = "VEGETATION_GROWTH";
            eventSubtype = ndviChange > 0.3m ? "REFORESTATION" : "MODERATE_GROWTH";
        }
        else if (ndwiChange > threshold)
        {
            eventType = "WATER_INCREASE";
            eventSubtype = ndwiChange > 0.3m ? "FLOODING" : "MODERATE_INCREASE";
        }
        else if (ndwiChange < -threshold)
        {
            eventType = "WATER_DECREASE";
            eventSubtype = ndwiChange < -0.3m ? "DROUGHT" : "MODERATE_DECREASE";
        }
        else if (ndbiChange > threshold)
        {
            eventType = "URBANIZATION";
            eventSubtype = "DEVELOPMENT";
        }

        return (eventType, eventSubtype, severity);
    }

    private decimal CalculateConfidence(decimal ndviChange, decimal ndwiChange)
    {
        var baseConfidence = 0.7m;
        var changeMagnitude = Math.Max(Math.Abs(ndviChange), Math.Abs(ndwiChange));
        return Math.Min(0.95m, baseConfidence + (changeMagnitude * 0.5m));
    }

    private string GenerateChangeDescription(string eventType, string eventSubtype, decimal ndviChange, decimal ndwiChange)
    {
        return $"{eventSubtype} - {eventType} detected. " +
               $"NDVI change: {ndviChange:F4}, NDWI change: {ndwiChange:F4}";
    }

    // Data access helper methods
    private async Task<List<string>> GetParcelsForAnalysisAsync(string? parcelUpid, string? region)
    {
        if (!string.IsNullOrEmpty(parcelUpid))
            return new List<string> { parcelUpid };

        var parcels = string.IsNullOrEmpty(region) ?
            await _context.ParcelViews.Select(p => p.Upid).ToListAsync() :
            await _context.ParcelViews.Where(p => p.Region == region).Select(p => p.Upid).ToListAsync();

        return parcels.Take(1000).ToList(); // Limit for performance
    }

    private async Task<List<string>> GetParcelsForProcessingAsync(string? parcelUpid)
    {
        return !string.IsNullOrEmpty(parcelUpid) 
            ? new List<string> { parcelUpid } 
            : await _context.ParcelViews.Select(p => p.Upid).Take(500).ToListAsync();
    }

    private async Task<Dictionary<string, decimal>?> CalculateSpectralIndicesForParcelAsync(string parcelUpid, DateTime date)
    {
        // In a real implementation, this would call satellite imagery processing services
        // For now, using mock data with realistic ranges
        var random = new Random();
        return new Dictionary<string, decimal>
        {
            ["NDVI"] = (decimal)(0.1 + random.NextDouble() * 0.8), // -0.1 to 1.0 typically
            ["NDWI"] = (decimal)(-0.5 + random.NextDouble() * 1.0), // -1.0 to 1.0
            ["NDBI"] = (decimal)(-1.0 + random.NextDouble() * 1.5), // -1.0 to 1.0
            ["EVI"] = (decimal)(random.NextDouble() * 2.0), // 0 to 2.0
            ["MNDWI"] = (decimal)(-1.0 + random.NextDouble() * 2.0), // -1.0 to 1.0
            ["VegetationHealth"] = (decimal)random.NextDouble(),
            ["WaterPresence"] = (decimal)random.NextDouble(),
            ["SoilMoisture"] = (decimal)random.NextDouble()
        };
    }

    private async Task StoreEnvironmentalMonitoringDataAsync(string parcelUpid, DateTime date, Dictionary<string, decimal> indices)
    {
        var geometry = await _parcelViewRepo.GetParcelGeometryAsync(parcelUpid);
        var area = await CalculateParcelAreaAsync(parcelUpid);

        var monitoringData = new ParcelEnvironmentalMonitoring
        {
            ParcelUpid = parcelUpid,
            MonitoringDate = date,
            NDVI = indices.GetValueOrDefault("NDVI"),
            NDWI = indices.GetValueOrDefault("NDWI"),
            NDBI = indices.GetValueOrDefault("NDBI"),
            EVI = indices.GetValueOrDefault("EVI"),
            MNDWI = indices.GetValueOrDefault("MNDWI"),
            VegetationHealth = indices.GetValueOrDefault("VegetationHealth"),
            WaterPresence = indices.GetValueOrDefault("WaterPresence"),
            SoilMoisture = indices.GetValueOrDefault("SoilMoisture"),
            Geometry = geometry,
            AreaSqkm = area,
            CloudCover = 0.1m,
            CreatedAt = DateTime.UtcNow
        };

        await _monitoringRepo.AddAsync(monitoringData);
    }

    private async Task<decimal> CalculateParcelAreaAsync(string parcelUpid)
    {
        var parcel = await _parcelViewRepo.GetByUpidAsync(parcelUpid);
        return parcel?.Area ?? 0;
    }

    private async Task StoreChangeEventsAsync(List<EnvironmentalChangeEventDto> changes)
    {
        foreach (var change in changes.Where(c => c.Confidence >= 0.6m))
        {
            var changeEvent = new EnvironmentalChangeEvent
            {
                ParcelUpid = change.ParcelUpid,
                EventDate = change.EventDate,
                EventType = change.EventType,
                EventSubtype = change.EventSubtype,
                BeforeValue = change.BeforeValue,
                AfterValue = change.AfterValue,
                ChangeAmount = change.ChangeAmount,
                ChangePercentage = change.ChangePercentage,
                AffectedAreaSqkm = change.AffectedArea,
                Severity = change.Severity,
                Confidence = change.Confidence,
                Geometry = CreateGeometryFromWkt(change.Geometry),
                Centroid = CreatePointFromWkt(change.Centroid),
                Description = change.Description,
                SatelliteEvidence = true,
                Verified = false,
                DetectedAt = DateTime.UtcNow
            };

            await _changeEventRepo.AddAsync(changeEvent);
        }

        await _changeEventRepo.SaveAsync();
        _logger.LogInformation("Stored {Count} change events to database", changes.Count(c => c.Confidence >= 0.6m));
    }

    // Utility methods
    private Dictionary<string, decimal> CreateSpectralIndicesDictionary(ParcelEnvironmentalMonitoring data)
    {
        var indices = new Dictionary<string, decimal>();
        if (data.NDVI.HasValue) indices["NDVI"] = data.NDVI.Value;
        if (data.NDWI.HasValue) indices["NDWI"] = data.NDWI.Value;
        if (data.NDBI.HasValue) indices["NDBI"] = data.NDBI.Value;
        if (data.EVI.HasValue) indices["EVI"] = data.EVI.Value;
        if (data.MNDWI.HasValue) indices["MNDWI"] = data.MNDWI.Value;
        if (data.VegetationHealth.HasValue) indices["VegetationHealth"] = data.VegetationHealth.Value;
        if (data.WaterPresence.HasValue) indices["WaterPresence"] = data.WaterPresence.Value;
        if (data.SoilMoisture.HasValue) indices["SoilMoisture"] = data.SoilMoisture.Value;
        return indices;
    }

    private EnvironmentalChangeEventDto MapToChangeEventDto(EnvironmentalChangeEvent e)
    {
        return new EnvironmentalChangeEventDto
        {
            ParcelUpid = e.ParcelUpid,
            EventDate = e.EventDate,
            EventType = e.EventType,
            EventSubtype = e.EventSubtype,
            BeforeValue = e.BeforeValue ?? 0,
            AfterValue = e.AfterValue ?? 0,
            ChangeAmount = e.ChangeAmount ?? 0,
            ChangePercentage = e.ChangePercentage ?? 0,
            AffectedArea = e.AffectedAreaSqkm ?? 0,
            Severity = e.Severity,
            Confidence = e.Confidence ?? 0,
            Geometry = e.Geometry?.AsText() ?? string.Empty,
            Centroid = e.Centroid?.AsText() ?? string.Empty,
            Description = e.Description
        };
    }

    private Geometry CreateGeometryFromWkt(string wkt)
    {
        try
        {
            var reader = new NetTopologySuite.IO.WKTReader();
            return string.IsNullOrEmpty(wkt) ? null : reader.Read(wkt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create geometry from WKT: {Wkt}", wkt);
            return null;
        }
    }

    private NetTopologySuite.Geometries.Point CreatePointFromWkt(string wkt)
    {
        try
        {
            var reader = new NetTopologySuite.IO.WKTReader();
            return string.IsNullOrEmpty(wkt) ? null : reader.Read(wkt) as NetTopologySuite.Geometries.Point;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create point from WKT: {Wkt}", wkt);
            return null;
        }
    }

    private Dictionary<string, int> GenerateChangeSummary(List<EnvironmentalChangeEventDto> changes)
    {
        return changes
            .GroupBy(c => c.EventType)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private Dictionary<string, decimal> CalculateAverageChanges(List<EnvironmentalChangeEventDto> changes)
    {
        var averages = new Dictionary<string, decimal>();

        if (changes.Any())
        {
            averages["NDVI"] = changes.Average(c => c.ChangeAmount);
            averages["AffectedArea"] = changes.Average(c => c.AffectedArea);
            averages["Confidence"] = changes.Average(c => c.Confidence);
        }

        return averages;
    }
}