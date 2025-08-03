using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Report.Models;
using System;
using System.Collections.Generic;

namespace intapscamis.camis.domain.Report
{
    public interface IReportFacade : ICamisFacade
    {

        void SetSession(UserSession session);
        ActivityPlan GetActivityPlan(Guid id);
        ActivityViewModel GetActivityViewModel(Guid id);
        List<ActivityPlanDetailViewModel> GetActivityPlanDetailViewModel(Guid id);
        PlanViewModel GetPlanViewModel(Guid planId);
        List<Activity> GetChildActivities(Guid id);

        List<TRegions> GetAllRegions();
        List<TZones> GetZones(string csaregionid);
        List<TWoredas> GetWoredas(string csaworedaid);
    }

    public class ReportFacade : CamisFacade , IReportFacade
    {
        private readonly IReportService _service;
        private UserSession _session;

        private CamisContext _context;

        protected ReportFacade(CamisContext context,IReportService service)
        {
            _context = context;
            _service = service;
        }

        public ActivityPlan GetActivityPlan(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetActivityPlan(id);
        }

        public List<ActivityPlanDetailViewModel> GetActivityPlanDetailViewModel(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetActivityPlanDetailViewModel(id);
        }

        public ActivityViewModel GetActivityViewModel(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetActivityViewModel(id);
        }

        public List<TRegions> GetAllRegions()
        {
            PassContext(_service, _context);
            return _service.GetAllRegions();
        }

        public List<Activity> GetChildActivities(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetChildActivities(id);
        }

        public PlanViewModel GetPlanViewModel(Guid planId)
        {
            PassContext(_service, _context);
            return _service.GetPlanViewModel(planId);
        }

        public List<TWoredas> GetWoredas(string csaworedaid)
        {
            PassContext(_service, _context);
            return _service.GetWoredas(csaworedaid);
        }

        public List<TZones> GetZones(string csaregionid)
        {
            PassContext(_service, _context);
            return _service.GetZones(csaregionid);
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _service.SetSession(_session);
        }

        
    }
}
