using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Repository;

public class ParcelViewRepository : IParcelViewRepository
{
    private readonly CamisContext _context;

    public ParcelViewRepository(CamisContext context)
    {
        _context = context;
    }

    public async Task<LandUpin> GetByUpidAsync(string upid)
    {
        return await _context.LandUpin
            .FirstOrDefaultAsync(p => p.Upin == upid);
    }

    public async Task<IEnumerable<LandUpin>> GetByRegionAsync(string region)
    {
        return await _context.LandUpin
            .FromSqlRaw(@"SELECT * FROM lb.land_upin WHERE profile->>'csaregionid' = '"+region+"'")
            .ToListAsync();
    }

    public async Task<IEnumerable<LandUpin>> GetWithinGeometryAsync(Geometry geometry)
    {
        return await _context.LandUpin
            .Where(p => p.Geometry != null && p.Geometry.Within(geometry))
            .ToListAsync();
    }

    public async Task<Geometry> GetParcelGeometryAsync(string upid)
    {
        var parcel = await _context.LandUpin
            .Where(p => p.Upin == upid).Select(p=>p.Geometry)
            .FirstOrDefaultAsync();
       
        return parcel;
    }

    public async Task<string> GetRegionBoundingBoxAsync(string region = null)
    {
        var query = _context.LandUpin.AsQueryable();

        if (!string.IsNullOrEmpty(region))
        {
            query = query.Where(p => p.Profile.Contains($"\"csaregionid\":\"{region}\""));
        }

        var extent = await query
            .Where(p => p.Geometry != null)
            .Select(p => p.Geometry.Envelope)
            .FirstOrDefaultAsync();

        if (extent == null)
            return "0,0,1,1"; // Default bbox

        return
            $"{extent.Coordinates[0].X},{extent.Coordinates[0].Y},{extent.Coordinates[2].X},{extent.Coordinates[2].Y}";
    }
}