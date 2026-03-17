using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.EnvironmentalMonitoring;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers;

public class EnvironmentalMonitoringController : BaseController
{
    private readonly IEnvironmentalMonitoringService _monitoringService;
    private readonly ILogger<EnvironmentalMonitoringController> _logger;

    public EnvironmentalMonitoringController(
        IEnvironmentalMonitoringService monitoringService, 
        ILogger<EnvironmentalMonitoringController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult> DetectChanges([FromBody] ChangeDetectionRequest request)
    {
        try
        {
            var result = await _monitoringService.AnalyzeEnvironmentalChangesAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting environmental changes");
            return StatusCode(500, "Error detecting environmental changes");
        }
    }

    [HttpPost]
    public async Task<ActionResult> ProcessImagery([FromBody] SpectralIndicesRequest request)
    {
        try
        {
            var result = await _monitoringService.ProcessSatelliteImageryAsync(request.Date, request.ParcelUpid);
            return result ? Ok("Imagery processed successfully") : BadRequest("Imagery processing failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing satellite imagery");
            return StatusCode(500, "Error processing satellite imagery");
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetParcelHistory([FromQuery]  string parcelUpid, int monthsBack = 12)
    {
        try
        {
            var history = await _monitoringService.GetParcelEnvironmentalHistoryAsync(parcelUpid, monthsBack);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parcel environmental history for {ParcelUpid}", parcelUpid);
            return StatusCode(500, "Error retrieving history");
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetSpectralIndices([FromQuery] string parcelUpid, DateTime date)
    {
        try
        {
            var indices = await _monitoringService.CalculateSpectralIndicesAsync(parcelUpid, date);
            return Ok(indices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating spectral indices for {ParcelUpid}", parcelUpid);
            return StatusCode(500, "Error calculating spectral indices");
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetSignificantChanges([FromQuery] DateTime startDate, DateTime endDate, string region = null)
    {
        try
        {
            var changes = await _monitoringService.GetSignificantChangesAsync(startDate, endDate, region);
            return Ok(changes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving significant changes from {StartDate} to {EndDate}", startDate, endDate);
            return StatusCode(500, "Error retrieving significant changes");
        }
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckFloodRisk([FromBody] string geometryWkt)
    {
        try
        {
            var floodRisk = await _monitoringService.MonitorFloodRiskAsync(geometryWkt);
            return Ok(floodRisk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing flood risk for geometry");
            return StatusCode(500, "Error assessing flood risk");
        }
    }

    [HttpGet]
    public async Task<ActionResult> CheckDroughtCondition([FromQuery] string region)
    {
        try
        {
            var droughtCondition = await _monitoringService.DetectDroughtConditionsAsync(region);
            return Ok(droughtCondition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting drought conditions for region {Region}", region);
            return StatusCode(500, "Error detecting drought conditions");
        }
    }

    // New endpoints based on the service capabilities
    [HttpGet]
    public async Task<ActionResult> GetParcelChanges([FromQuery] string parcelUpid, DateTime startDate, DateTime endDate, decimal threshold = 0.15m)
    {
        try
        {
            var request = new ChangeDetectionRequest
            {
                ParcelUpid = parcelUpid,
                StartDate = startDate,
                EndDate = endDate,
                ChangeThreshold = threshold
            };

            var result = await _monitoringService.AnalyzeEnvironmentalChangesAsync(request);
            return Ok(result.Changes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving changes for parcel {ParcelUpid}", parcelUpid);
            return StatusCode(500, "Error retrieving parcel changes");
        }
    }

    [HttpGet]
    public async Task<ActionResult> HealthCheck()
    {
        try
        {
            // Simple health check by trying to retrieve recent data
            var recentDate = DateTime.UtcNow.AddDays(-1);
            var testResult = await _monitoringService.GetSignificantChangesAsync(
                recentDate, DateTime.UtcNow);

            return Ok($"Service is healthy. Found {testResult.Count} recent changes.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, "Service unavailable");
        }
    }
}