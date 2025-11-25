using intapscamis.camis.data.Entities;

namespace intapscamis.camis.domain.EnvironmentalMonitoring.Interface;

public interface IEnvironmentalChangeEventRepository: IRepository<EnvironmentalChangeEvent>
{
    Task<IEnumerable<EnvironmentalChangeEvent>> GetByParcelAsync(string parcelUpid, int monthsBack = 12);
    Task<IEnumerable<EnvironmentalChangeEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<EnvironmentalChangeEvent>> GetSignificantEventsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<EnvironmentalChangeEvent>> GetByEventTypeAsync(string eventType, DateTime startDate, DateTime endDate);
}