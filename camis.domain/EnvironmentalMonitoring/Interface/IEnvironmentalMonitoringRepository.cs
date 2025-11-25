using intapscamis.camis.data.Entities;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface IEnvironmentalMonitoringRepository: IRepository<ParcelEnvironmentalMonitoring>
{
    Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetByParcelAndDateRangeAsync(string parcelUpid, DateTime startDate, DateTime endDate);
    Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetByDateAsync(DateTime date);
    Task<ParcelEnvironmentalMonitoring?> GetLatestByParcelAsync(string parcelUpid);
    Task<IEnumerable<string>> GetParcelsWithChangesAsync(DateTime startDate, DateTime endDate, decimal threshold = 0.15m);
    Task<IEnumerable<ParcelEnvironmentalMonitoring>> GetSignificantChangesAsync(DateTime startDate, DateTime endDate);
}