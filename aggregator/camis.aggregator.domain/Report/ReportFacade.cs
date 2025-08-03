using camis.aggregator.data.Entities;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Farms.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace camis.aggregator.domain.Report
{
    public interface IReportFacade : ICamisFacade
    {
        void SetSession(UserSession session);


        List<TRegions> GetAllRegions();
        List<TZones> GetZones(string csaregionid);
        List<TWoredas> GetWoredas(string csaworedaid);
        void SetRegionUrl(string regionId, string regionUrl, string username, string password);
        void UpdateRegionConfig(RegionConfigModel Model);
        TRegions GetRegion(string code);
        ReportResponseModel GetReport(ReportRequestModel Request);
    }

    public class ReportFacade : CamisFacade, IReportFacade
    {

        private readonly IReportService _service;
        private UserSession _session;

        private aggregatorContext _context;

        protected ReportFacade(aggregatorContext context, IReportService service)
        {
            _context = context;
            _service = service;
        }


        public List<TRegions> GetAllRegions()
        {
            PassContext(_service, _context);
            return _service.GetAllRegions();
        }

        public TRegions GetRegion(string code)
        {
            PassContext(_service, _context);
            return _service.GetRegion(code);
        }

        public ReportResponseModel GetReport(ReportRequestModel Request)
        {
            PassContext(_service, _context);
            return _service.GetReport(Request);
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

        public void SetRegionUrl(string regionId, string regionUrl, string username, string password)
        {
            Transact( _context, t =>
            {
                PassContext(_service, _context);
                _service.SetRegionUrl(regionId, regionUrl,username,password);

            });
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _service.SetSession(_session);
        }

        public void UpdateRegionConfig(RegionConfigModel Model)
        {
            Transact(_context, t =>
            {
                PassContext(_service, _context);
                _service.UpdateRegionConfig(Model);
            });
        }
    }
}
