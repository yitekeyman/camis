using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.EnvironmentalMonitoring.Interface;
using Microsoft.EntityFrameworkCore;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Repository;

public class EnvironmentalChangeEventRepository: IEnvironmentalChangeEventRepository
{
    private readonly CamisContext _context;
    private readonly DbSet<EnvironmentalChangeEvent> _dbSet;

    public EnvironmentalChangeEventRepository(CamisContext context)
    {
        _context = context;
        _dbSet = context.Set<EnvironmentalChangeEvent>();
    }

    public async Task<EnvironmentalChangeEvent?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<EnvironmentalChangeEvent>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(EnvironmentalChangeEvent entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(EnvironmentalChangeEvent entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(EnvironmentalChangeEvent entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<EnvironmentalChangeEvent>> GetByParcelAsync(string parcelUpid, int monthsBack = 12)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-monthsBack);
        return await _dbSet
            .Where(e => e.ParcelUpid == parcelUpid && e.EventDate >= cutoffDate)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EnvironmentalChangeEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.EventDate >= startDate && e.EventDate <= endDate)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EnvironmentalChangeEvent>> GetSignificantEventsAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.EventDate >= startDate && 
                       e.EventDate <= endDate && 
                       e.Confidence >= 0.7m && 
                       (e.Severity == "HIGH" || e.Severity == "SEVERE"))
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<EnvironmentalChangeEvent>> GetByEventTypeAsync(string eventType, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.EventType == eventType && 
                       e.EventDate >= startDate && 
                       e.EventDate <= endDate)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
    }
    
}