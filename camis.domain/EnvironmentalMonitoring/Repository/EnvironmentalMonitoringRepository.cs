using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.EntityFrameworkCore;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Repository;

public class EnvironmentalMonitoringRepository: IEnvironmentalMonitoringRepository
{
    private readonly CamisContext _context;
    private readonly DbSet<ParcelEnvironmentalMonitoring> _dbSet;
    
    public EnvironmentalMonitoringRepository(CamisContext context)
    {
        _context = context;
        _dbSet = context.Set<ParcelEnvironmentalMonitoring>();
    }
        public async Task<ParcelEnvironmentalMonitoring> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(ParcelEnvironmentalMonitoring entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(ParcelEnvironmentalMonitoring entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(ParcelEnvironmentalMonitoring entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetByParcelAndDateRangeAsync(string parcelUpid, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.ParcelUpid == parcelUpid && 
                       e.MonitoringDate >= startDate && 
                       e.MonitoringDate <= endDate)
            .OrderBy(e => e.MonitoringDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetByDateAsync(DateTime date)
    {
        return await _dbSet
            .Where(e => e.MonitoringDate.Date == date.Date)
            .ToListAsync();
    }

    public async Task<ParcelEnvironmentalMonitoring> GetLatestByParcelAsync(string parcelUpid)
    {
        return await _dbSet
            .Where(e => e.ParcelUpid == parcelUpid)
            .OrderByDescending(e => e.MonitoringDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetParcelsWithChangesAsync(DateTime startDate, DateTime endDate, decimal threshold = 0.15m)
    {
        return await _dbSet
            .Where(e => e.MonitoringDate >= startDate && 
                       e.MonitoringDate <= endDate && 
                       e.ChangeMagnitude.HasValue && 
                       Math.Abs(e.ChangeMagnitude.Value) >= threshold)
            .Select(e => e.ParcelUpid)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetSignificantChangesAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.MonitoringDate >= startDate && 
                       e.MonitoringDate <= endDate && 
                       e.Confidence >= 0.7m && 
                       e.Severity != "LOW")
            .OrderByDescending(e => e.ChangeMagnitude)
            .ToListAsync();
    }

}