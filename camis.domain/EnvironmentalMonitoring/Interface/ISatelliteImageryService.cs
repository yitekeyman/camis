using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface ISatelliteImageryService
{
    Task<SatelliteImage> GetImageryAsync(Geometry geometry, DateTime date);
    Task<Dictionary<string, decimal>> CalculateSpectralIndicesAsync(SatelliteImage image);
    Task<List<DateTime>> GetAvailableDatesAsync(Geometry geometry, DateTime start, DateTime end);
    Task<bool> IsImageCloudFreeAsync(SatelliteImage image);
}