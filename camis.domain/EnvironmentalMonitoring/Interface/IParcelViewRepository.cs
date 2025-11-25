using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface IParcelViewRepository
{
    Task<ParcelView?> GetByUpidAsync(string upid);
    Task<IEnumerable<ParcelView>> GetByRegionAsync(string region);
    Task<IEnumerable<ParcelView>> GetWithinGeometryAsync(Geometry geometry);
    Task<Geometry?> GetParcelGeometryAsync(string upid);
    Task<string> GetRegionBoundingBoxAsync(string? region = null);
}