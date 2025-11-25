using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.EnvironmentalMonitoring;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using intapscamis.camis.domain.EnvironmentalMonitoring.Repository;
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

    public async Task<ParcelView?> GetByUpidAsync(string upid)
    {
        return await _context.ParcelViews
            .FirstOrDefaultAsync(p => p.Upid == upid);
    }

    public async Task<IEnumerable<ParcelView>> GetByRegionAsync(string region)
    {
        return await _context.ParcelViews
            .Where(p => p.Region == region)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParcelView>> GetWithinGeometryAsync(Geometry geometry)
    {
        return await _context.ParcelViews
            .Where(p => p.Geometry != null && p.Geometry.Within(geometry))
            .ToListAsync();
    }

    public async Task<Geometry?> GetParcelGeometryAsync(string upid)
    {
        var parcel = await _context.ParcelViews
            .Where(p => p.Upid == upid)
            .Select(p => p.Geometry)
            .FirstOrDefaultAsync();
        return parcel;
    }

    public async Task<string> GetRegionBoundingBoxAsync(string? region = null)
    {
        var query = _context.ParcelViews.AsQueryable();

        if (!string.IsNullOrEmpty(region))
        {
            query = query.Where(p => p.Region == region);
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