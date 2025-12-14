using intapscamis.camis.data.Entities;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface IParcelViewRepository
{
    Task<LandUpin?> GetByUpidAsync(string upid);
    Task<IEnumerable<LandUpin>> GetByRegionAsync(string region);
    Task<IEnumerable<LandUpin>> GetWithinGeometryAsync(Geometry geometry);
    Task<Geometry?> GetParcelGeometryAsync(string upid);
    Task<string> GetRegionBoundingBoxAsync(string? region = null);
}