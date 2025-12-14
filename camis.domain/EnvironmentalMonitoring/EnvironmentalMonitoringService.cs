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
    private readonly ISatelliteServiceFactory _satelliteServiceFactory;

    public EnvironmentalMonitoringService(
        IEnvironmentalMonitoringRepository monitoringRepo,
        IEnvironmentalChangeEventRepository changeEventRepo,
        IParcelViewRepository parcelViewRepo,
        IGeoServerService geoserver,
        CamisContext context,
        ILogger<EnvironmentalMonitoringService> logger,
        ISatelliteServiceFactory satelliteServiceFactory)
    {
        _monitoringRepo = monitoringRepo;
        _changeEventRepo = changeEventRepo;
        _parcelViewRepo = parcelViewRepo;
        _geoServer = geoserver;
        _logger = logger;
        _context = context;
        _satelliteServiceFactory = satelliteServiceFactory;
    }

    public async Task<EnvironmentalAnalysisResult> AnalyzeEnvironmentalChangesAsync(ChangeDetectionRequest request)
    {
        _logger.LogInformation("Starting environmental change analysis for {StartDate} to {EndDate}",
            request.StartDate, request.EndDate);

        var result = new EnvironmentalAnalysisResult();

        try
        {
            // Use real satellite service for production, simulated for testing
            var useRealService = ShouldUseRealSatelliteService(request);

            // Process imagery if needed
            if (!string.IsNullOrEmpty(request.ParcelUpid))
            {
                await ProcessSatelliteImageryAsync(request.EndDate, request.ParcelUpid, useRealService);
            }

            // Detect changes using multiple algorithms
            var changes = await DetectChangesWithMultipleAlgorithmsAsync(request);
            result.Changes = changes;
            result.ChangeSummary = GenerateChangeSummary(changes);
            result.TotalAffectedArea = changes.Sum(c => c.AffectedArea);
            result.AverageChanges = CalculateAverageChanges(changes);

            // Publish to GeoServer if changes found
            if (changes.Any())
            {
                var publishResult = await _geoServer.PublishEnvironmentalChangesAsync(changes);
                if (publishResult.Success)
                {
                    var bbox = await _parcelViewRepo.GetRegionBoundingBoxAsync(request.Region);
                    result.ChangeMapUrl = await _geoServer.BuildEnvironmentalChangesWmsUrl(bbox);
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

    public async Task<bool> ProcessSatelliteImageryAsync(DateTime date, string? parcelUpid = null,
        bool useRealService = false)
    {
        try
        {
            var satelliteService = _satelliteServiceFactory.GetSatelliteService(useRealService);
            var parcels = await GetParcelsForProcessingAsync(parcelUpid);

            _logger.LogInformation("Processing satellite imagery for {Count} parcels on {Date} using {Service}",
                parcels.Count, date, useRealService ? "SentinelHub" : "Simulated");

            foreach (var parcel in parcels)
            {
                var spectralData = await CalculateSpectralIndicesForParcelAsync(parcel, date, satelliteService);
                if (spectralData != null)
                {
                    await StoreEnvironmentalMonitoringDataAsync(parcel, date, spectralData);
                }
            }

            await _monitoringRepo.SaveAsync();
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

    public async Task<List<ParcelEnvironmentalMonitoring>> GetParcelEnvironmentalHistoryAsync(string parcelUpid,
        int monthsBack = 12)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-monthsBack);
        return (await _monitoringRepo.GetByParcelAndDateRangeAsync(parcelUpid, cutoffDate, DateTime.UtcNow))
            .ToList();
    }

    public async Task<List<EnvironmentalChangeEventDto>> GetSignificantChangesAsync(DateTime startDate,
        DateTime endDate, string? region = null)
    {
        var changeEvents = await _changeEventRepo.GetSignificantEventsAsync(startDate, endDate);
        return changeEvents.Select(MapToChangeEventDto).ToList();
    }

    public async Task<bool> MonitorFloodRiskAsync(string geometryWkt)
    {
        try
        {
            var geometry = CreateGeometryFromWkt(geometryWkt);
            var parcels = await _parcelViewRepo.GetWithinGeometryAsync(geometry);

            var parcelUpids = parcels.Select(p => p.Upin).ToList();
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
            var parcelUpids = parcels.Select(p => p.Upin).ToList();

            var recentData = await _monitoringRepo.GetByDateAsync(DateTime.UtcNow.AddDays(-30));
            var regionData = recentData.Where(d => parcelUpids.Contains(d.ParcelUpid)).ToList();

            var avgNdvi = regionData.Average(d => d.NDVI) ?? 0;
            var avgNdwi = regionData.Average(d => d.NDWI) ?? 0;
            var avgMoisture = regionData.Average(d => d.SoilMoisture) ?? 0;

            bool droughtConditions = avgNdvi < 0.3m && avgNdwi < 0.1m && avgMoisture < 0.2m;

            _logger.LogInformation("Drought conditions assessment for {Region}: {Conditions}",
                region, droughtConditions ? "DROUGHT" : "NORMAL");

            return droughtConditions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting drought conditions");
            return false;
        }
    }

    public async Task<bool> RunScheduledChangeDetectionAsync()
    {
        try
        {
            _logger.LogInformation("Starting scheduled change detection");

            var parcels = await GetParcelsForProcessingAsync(null);
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-30);

            foreach (var parcel in parcels)
            {
                await ProcessSatelliteImageryAsync(endDate, parcel);

                var request = new ChangeDetectionRequest
                {
                    ParcelUpid = parcel,
                    StartDate = startDate,
                    EndDate = endDate,
                    ChangeThreshold = 0.15m
                };

                var changes = await DetectChangesWithStatisticsAsync(request);
                if (changes.Any())
                {
                    await StoreChangeEventsAsync(changes);
                }
            }

            _logger.LogInformation("Scheduled change detection completed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in scheduled change detection");
            return false;
        }
    }

    // Private helper methods
    private async Task<Dictionary<string, decimal>?> CalculateSpectralIndicesForParcelAsync(
        string parcelUpid, DateTime date, ISatelliteImageryService satelliteService)
    {
        try
        {
            var geometry = await _parcelViewRepo.GetParcelGeometryAsync(parcelUpid);
            if (geometry == null)
            {
                _logger.LogWarning("No geometry found for parcel {ParcelUpid}", parcelUpid);
                return null;
            }

            var satelliteImage = await satelliteService.GetImageryAsync(geometry, date);
            if (satelliteImage == null)
            {
                _logger.LogWarning("No satellite imagery available for parcel {ParcelUpid} on {Date}", parcelUpid,
                    date);
                return null;
            }

            if (!await satelliteService.IsImageCloudFreeAsync(satelliteImage))
            {
                _logger.LogWarning("Image for parcel {ParcelUpid} on {Date} has cloud cover", parcelUpid, date);
                return null;
            }

            var indices = await satelliteService.CalculateSpectralIndicesAsync(satelliteImage);
            return indices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating spectral indices for parcel {ParcelUpid}", parcelUpid);
            return null;
        }
    }

    private bool ShouldUseRealSatelliteService(ChangeDetectionRequest request)
    {
        // Use real service for recent dates and important analyses
        return request.EndDate > DateTime.UtcNow.AddMonths(-3) ||
               !string.IsNullOrEmpty(request.ParcelUpid);
    }

    private async Task<List<EnvironmentalChangeEventDto>> DetectChangesWithMultipleAlgorithmsAsync(
        ChangeDetectionRequest request)
    {
        var changes = new List<EnvironmentalChangeEventDto>();
        var parcels = await GetParcelsForAnalysisAsync(request.ParcelUpid, request.Region);

        foreach (var parcel in parcels)
        {
            var thresholdChanges =
                await AnalyzeParcelChangesAsync(parcel, request.StartDate, request.EndDate, request.ChangeThreshold);
            var statisticalChanges = await AnalyzeParcelWithStatisticalSignificanceAsync(parcel, request);
            var anomalyChanges = await DetectAnomaliesWithZScoreAsync(parcel, request);

            var allChanges = thresholdChanges
                .Concat(statisticalChanges)
                .Concat(anomalyChanges)
                .GroupBy(c => new { c.ParcelUpid, c.EventDate.Date, c.EventType })
                .Select(g => g.OrderByDescending(c => c.Confidence).First())
                .ToList();

            changes.AddRange(allChanges);
        }

        return changes;
    }

    private async Task<List<EnvironmentalChangeEventDto>> DetectChangesWithStatisticsAsync(
        ChangeDetectionRequest request)
    {
        var changes = new List<EnvironmentalChangeEventDto>();
        var parcels = await GetParcelsForAnalysisAsync(request.ParcelUpid, request.Region);

        foreach (var parcel in parcels)
        {
            var parcelChanges = await AnalyzeParcelWithStatisticalSignificanceAsync(parcel, request);
            changes.AddRange(parcelChanges);
        }

        return changes;
    }


    private async Task<List<EnvironmentalChangeEventDto>> AnalyzeParcelWithStatisticalSignificanceAsync(
        string parcelUpid, ChangeDetectionRequest request)
    {
        var monitoringData =
            (await _monitoringRepo.GetByParcelAndDateRangeAsync(parcelUpid, request.StartDate, request.EndDate))
            .OrderBy(m => m.MonitoringDate)
            .ToList();

        if (monitoringData.Count < 2)
            return new List<EnvironmentalChangeEventDto>();

        var changes = new List<EnvironmentalChangeEventDto>();
        var indices = new[] { "NDVI", "NDWI", "NDBI", "MNDWI" };

        foreach (var index in indices)
        {
            var indexChanges = await DetectIndexChangeAsync(parcelUpid, monitoringData, index, request.ChangeThreshold);
            changes.AddRange(indexChanges);
        }

        return changes;
    }

    private async Task<List<EnvironmentalChangeEventDto>> DetectIndexChangeAsync(
        string parcelUpid, List<ParcelEnvironmentalMonitoring> data, string index, decimal threshold)
    {
        var changes = new List<EnvironmentalChangeEventDto>();

        // Extract time series for the specific index
        var timeSeries = data
            .Select(m => new TimeSeriesPoint
            {
                Date = m.MonitoringDate,
                Value = GetIndexValue(m, index) ?? 0 // Handle null values
            })
            .Where(x => x.Value != 0) // Filter out null/zero values
            .ToList();

        if (timeSeries.Count < 2) return changes;

        // Calculate trend using linear regression
        var trend = CalculateTrend(timeSeries);

        // Check if trend is statistically significant
        if (Math.Abs(trend.Slope) > threshold && trend.Confidence > 0.7m)
        {
            var changeEvent = await CreateStatisticalChangeEventAsync(
                parcelUpid, index, timeSeries, trend, threshold);
            if (changeEvent != null)
                changes.Add(changeEvent);
        }

        return changes;
    }

    private (decimal Slope, decimal Confidence) CalculateTrend(List<TimeSeriesPoint> timeSeries)
    {
        if (timeSeries == null || timeSeries.Count < 2)
            return (0, 0);

        var n = timeSeries.Count;

        // Convert dates to numeric values (days since first date)
        var firstDate = timeSeries.Min(t => t.Date);
        var dates = timeSeries.Select(t => (decimal)(t.Date - firstDate).TotalDays).ToArray();
        var values = timeSeries.Select(t => t.Value).ToArray();

        // Calculate means
        var meanX = dates.Average();
        var meanY = values.Average();

        // Calculate slope (m) for y = mx + b
        decimal numerator = 0;
        decimal denominator = 0;

        for (int i = 0; i < n; i++)
        {
            numerator += (dates[i] - meanX) * (values[i] - meanY);
            denominator += (dates[i] - meanX) * (dates[i] - meanX);
        }

        if (denominator == 0) return (0, 0);

        var slope = numerator / denominator;

        // Calculate R-squared for confidence
        decimal totalSumSquares = 0;
        decimal residualSumSquares = 0;

        for (int i = 0; i < n; i++)
        {
            totalSumSquares += (values[i] - meanY) * (values[i] - meanY);

            var predicted = slope * dates[i] + (meanY - slope * meanX);
            residualSumSquares += (values[i] - predicted) * (values[i] - predicted);
        }

        if (totalSumSquares == 0) return (slope, 0);

        var rSquared = 1 - (residualSumSquares / totalSumSquares);
        var confidence = Math.Max(0, Math.Min(1, rSquared));

        return (slope, confidence);
    }

    private decimal? GetIndexValue(ParcelEnvironmentalMonitoring data, string index)
    {
        return index switch
        {
            "NDVI" => data.NDVI,
            "NDWI" => data.NDWI,
            "NDBI" => data.NDBI,
            "MNDWI" => data.MNDWI,
            "EVI" => data.EVI,
            _ => null
        };
    }

    private async Task<EnvironmentalChangeEventDto> CreateStatisticalChangeEventAsync(
        string parcelUpid, string index, List<TimeSeriesPoint> timeSeries,
        (decimal Slope, decimal Confidence) trend, decimal threshold)
    {
        var first = timeSeries.First();
        var last = timeSeries.Last();
        var changeAmount = last.Value - first.Value;

        var (eventType, eventSubtype, severity) = ClassifyStatisticalChange(index, trend.Slope, threshold);

        var geometry = await _parcelViewRepo.GetParcelGeometryAsync(parcelUpid);
        var area = await CalculateParcelAreaAsync(parcelUpid);

        return new EnvironmentalChangeEventDto
        {
            ParcelUpid = parcelUpid,
            EventDate = last.Date,
            EventType = eventType,
            EventSubtype = eventSubtype,
            BeforeValue = first.Value,
            AfterValue = last.Value,
            ChangeAmount = changeAmount,
            ChangePercentage = first.Value != 0 ? (changeAmount / first.Value) * 100 : 0,
            AffectedArea = (decimal)area,
            Severity = severity,
            Confidence = trend.Confidence,
            Geometry = geometry?.AsText() ?? string.Empty,
            Centroid = geometry?.Centroid?.AsText() ?? string.Empty,
            Description =
                $"Statistical {index} change detected. Trend: {trend.Slope:F4}, Confidence: {trend.Confidence:P0}"
        };
    }

    private (string EventType, string EventSubtype, string Severity) ClassifyStatisticalChange(
        string index, decimal slope, decimal threshold)
    {
        var absSlope = Math.Abs(slope);
        var severity = absSlope > threshold * 2 ? "HIGH" :
            absSlope > threshold * 1.5m ? "MODERATE" : "LOW";

        var eventType = index switch
        {
            "NDVI" => slope > 0 ? "VEGETATION_GROWTH" : "VEGETATION_LOSS",
            "NDWI" => slope > 0 ? "WATER_INCREASE" : "WATER_DECREASE",
            "NDBI" => slope > 0 ? "URBANIZATION" : "DE_URBANIZATION",
            "MNDWI" => slope > 0 ? "WATER_INCREASE" : "WATER_DECREASE",
            _ => "ENVIRONMENTAL_CHANGE"
        };

        var eventSubtype = slope > 0 ? "INCREASING_TREND" : "DECREASING_TREND";

        return (eventType, eventSubtype, severity);
    }

    private async Task<List<EnvironmentalChangeEventDto>> DetectChangesUsingEntityFrameworkAsync(
        ChangeDetectionRequest request)
    {
        var changes = new List<EnvironmentalChangeEventDto>();
        var parcels = await GetParcelsForAnalysisAsync(request.ParcelUpid, request.Region);

        _logger.LogInformation("Analyzing changes for {Count} parcels", parcels.Count);

        foreach (var parcel in parcels)
        {
            var parcelChanges =
                await AnalyzeParcelChangesAsync(parcel, request.StartDate, request.EndDate, request.ChangeThreshold);
            changes.AddRange(parcelChanges);
        }

        // Filter by change types if specified
        if (request.ChangeTypes?.Any() == true)
        {
            changes = changes.Where(c => request.ChangeTypes.Contains(c.EventType)).ToList();
        }

        return changes;
    }

    private async Task<List<EnvironmentalChangeEventDto>> AnalyzeParcelChangesAsync(string parcelUpid,
        DateTime startDate, DateTime endDate, decimal threshold)
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
            var changeEvent = await CreateChangeEventAsync(parcelUpid, startData, endData, ndviChange, ndwiChange,
                ndbiChange, threshold);
            return changeEvent != null
                ? new List<EnvironmentalChangeEventDto> { changeEvent }
                : new List<EnvironmentalChangeEventDto>();
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
            ChangePercentage = startData.NDVI.HasValue && startData.NDVI.Value != 0
                ? (ndviChange / startData.NDVI.Value) * 100
                : 0,
            AffectedArea = (decimal)area,
            Severity = severity,
            Confidence = CalculateConfidence(ndviChange, ndwiChange),
            Geometry = geometry?.AsText() ?? string.Empty,
            Centroid = geometry?.Centroid?.AsText() ?? string.Empty,
            Description = GenerateChangeDescription(eventType, eventSubtype, ndviChange, ndwiChange)
        };
    }

    private (string EventType, string EventSubtype, string Severity) ClassifyChange(decimal ndviChange,
        decimal ndwiChange, decimal ndbiChange, decimal threshold)
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

    private string GenerateChangeDescription(string eventType, string eventSubtype, decimal ndviChange,
        decimal ndwiChange)
    {
        return $"{eventSubtype} - {eventType} detected. " +
               $"NDVI change: {ndviChange:F4}, NDWI change: {ndwiChange:F4}";
    }

    // Data access helper methods
    private async Task<List<string>> GetParcelsForAnalysisAsync(string? parcelUpid, string? region)
    {
        if (!string.IsNullOrEmpty(parcelUpid))
            return new List<string> { parcelUpid };

        var parcels = string.IsNullOrEmpty(region)
            ? await _context.ParcelViews.Select(p => p.Upid).ToListAsync()
            : await _context.ParcelViews.Where(p => p.Region == region).Select(p => p.Upid).ToListAsync();

        return parcels.Take(1000).ToList();
    }


    private async Task<List<string>> GetParcelsForProcessingAsync(string? parcelUpid)
    {
        return !string.IsNullOrEmpty(parcelUpid)
            ? new List<string> { parcelUpid }
            : await _context.ParcelViews.Select(p => p.Upid).Take(500).ToListAsync();
    }


    private async Task StoreEnvironmentalMonitoringDataAsync(string parcelUpid, DateTime date,
        Dictionary<string, decimal> indices)
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
            AreaSqkm = (decimal)area,
            CloudCover = 0.1m,
            CreatedAt = DateTime.UtcNow
        };

        await _monitoringRepo.AddAsync(monitoringData);
    }

    private async Task<double> CalculateParcelAreaAsync(string parcelUpid)
    {
        var parcel = await _parcelViewRepo.GetByUpidAsync(parcelUpid);
        return parcel?.Area??0;
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
        return indices;
    }

    private Geometry CreateGeometryFromWkt(string wkt)
    {
        try
        {
            var reader = new WKTReader();
            return string.IsNullOrEmpty(wkt) ? null : reader.Read(wkt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create geometry from WKT");
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

    private async Task<List<EnvironmentalChangeEventDto>> DetectAnomaliesWithZScoreAsync(string parcelUpid,
        ChangeDetectionRequest request)
    {
        var changes = new List<EnvironmentalChangeEventDto>();
        var monitoringData = (await _monitoringRepo.GetByParcelAndDateRangeAsync(
                parcelUpid, request.StartDate.AddMonths(-6), request.EndDate)) // Use longer history for baseline
            .OrderBy(m => m.MonitoringDate)
            .ToList();

        if (monitoringData.Count < 10) // Need sufficient data for anomaly detection
            return changes;

        var indices = new[] { "NDVI", "NDWI", "NDBI" };

        foreach (var index in indices)
        {
            var timeSeries = monitoringData
                .Select(m => new TimeSeriesPoint
                {
                    Date = m.MonitoringDate,
                    Value = GetIndexValue(m, index) ?? 0
                })
                .Where(x => x.Value != 0)
                .ToList();

            var anomalies = DetectAnomalies(timeSeries, request.ChangeThreshold);
            changes.AddRange(await CreateAnomalyEventsAsync(parcelUpid, index, anomalies));
        }

        return changes;
    }

    private List<TimeSeriesPoint> DetectAnomalies(List<TimeSeriesPoint> timeSeries, decimal threshold)
    {
        var anomalies = new List<TimeSeriesPoint>();

        if (timeSeries.Count < 10) return anomalies;
        // Calculate moving average and standard deviation
        var windowSize = Math.Min(5, timeSeries.Count / 2);

        for (int i = windowSize; i < timeSeries.Count; i++)
        {
            var window = timeSeries.Skip(i - windowSize).Take(windowSize).ToList();
            var mean = window.Average(t => t.Value);
            var stdDev = CalculateStandardDeviation(window.Select(t => t.Value));

            var currentValue = timeSeries[i].Value;
            var zScore = stdDev != 0 ? Math.Abs((currentValue - mean) / stdDev) : 0;

            if (zScore > (decimal)threshold * 2) // Anomaly if z-score > 2*threshold
            {
                anomalies.Add(timeSeries[i]);
            }
        }

        return anomalies;
    }

    private decimal CalculateStandardDeviation(IEnumerable<decimal> values)
    {
        var valueList = values.ToList();
        var mean = valueList.Average();
        var sumOfSquares = valueList.Sum(v => (v - mean) * (v - mean));
        return (decimal)Math.Sqrt((double)(sumOfSquares / valueList.Count));
    }

    private async Task<List<EnvironmentalChangeEventDto>> CreateAnomalyEventsAsync(string parcelUpid, string index,
        List<TimeSeriesPoint> anomalies)
    {
        var changes = new List<EnvironmentalChangeEventDto>();

        foreach (var anomaly in anomalies)
        {
            var geometry = await _parcelViewRepo.GetParcelGeometryAsync(parcelUpid);
            var area = await CalculateParcelAreaAsync(parcelUpid);

            var changeEvent = new EnvironmentalChangeEventDto
            {
                ParcelUpid = parcelUpid,
                EventDate = anomaly.Date,
                EventType = $"{index}_ANOMALY",
                EventSubtype = "STATISTICAL_OUTLIER",
                BeforeValue = 0, // Would need baseline for comparison
                AfterValue = anomaly.Value,
                ChangeAmount = anomaly.Value,
                ChangePercentage = 0,
                AffectedArea = (decimal)area,
                Severity = "HIGH",
                Confidence = 0.8m,
                Geometry = geometry?.AsText() ?? string.Empty,
                Centroid = geometry?.Centroid?.AsText() ?? string.Empty,
                Description = $"{index} anomaly detected with value {anomaly.Value:F4}"
            };

            changes.Add(changeEvent);
        }

        return changes;
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
}