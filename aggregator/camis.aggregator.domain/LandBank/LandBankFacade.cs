using camis.aggregator.data.Entities;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.domain.Infrastructure.Architecture;
using camis.types.Utils;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.LandBankGood.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace camis.aggregator.domain.LandBank
{
    public interface ILandBankFacade : ICamisFacade
    {
        void SetSession(UserSession session);
        Guid RegisterLand(LandBankFacadeModel.LandData data);
        void Updateland(LandBankFacadeModel.LandData data);
        LandBankFacadeModel.LandData GetLandByUpin(string u);
        LandBankFacadeModel.LandData GetLand(Guid landID, bool geom, bool dd);
        void RemoveLand(Guid landID);
        void CalculateCentroid(LandBankFacadeModel.LandData land);

        List<LandUpin> GetLandList();
        List<LandUpin> GetLandList(string region);
        InitLandData GetInitData();
        string SynchronizeLand(string[] regions);
        LandBankFacadeModel.Bound GetLandMapBound();
    }

    public class LandBankFacade : CamisFacade, ILandBankFacade
    {
        private readonly ILandBankService _service;
        private UserSession _session;

        private aggregatorContext _context;

        public LandBankFacade(aggregatorContext context, ILandBankService service)
        {
            _context = context;
            _service = service;
        }

        public void CalculateCentroid(LandBankFacadeModel.LandData land)
        {
            Transact(_context, t =>
           {
               PassContext(_service, _context);
               _service.CalculateCentroid(land);
           })
        ;
        }

        public InitLandData GetInitData()
        {
            PassContext(_service, _context);
            return _service.GetInitData();
        }

        public LandBankFacadeModel.LandData GetLand(Guid landID, bool geom, bool dd)
        {
            PassContext(_service, _context);
            return _service.GetLand(landID, geom, dd);
        }

        public LandBankFacadeModel.LandData GetLandByUpin(string u)
        {
            PassContext(_service, _context);
            return GetLandByUpin(u);
        }

        public List<LandUpin> GetLandList()
        {
            PassContext(_service, _context);
            return _service.GetLandList();
        }

        public List<LandUpin> GetLandList(string region)
        {
            PassContext(_service, _context);
            return _service.GetLandList(region);
        }

        public Guid RegisterLand(LandBankFacadeModel.LandData data)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
               return  _service.RegisterLand(data);
            });
        }

        public void RemoveLand(Guid landID)
        {
            Transact(_context, t =>
            {
                PassContext(_service, _context);
                _service.RemoveLand(landID);
            })
        ;
        }

        public void SetSession(UserSession session)
        {
                _session = session;
                _service.SetSession(_session);
            
        }

        public string SynchronizeLand(string[] regions)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.SynchronizeLand(regions);
            });
        }

        public void Updateland(LandBankFacadeModel.LandData data)
        {
            Transact(_context, t =>
            {
                PassContext(_service, _context);
                _service.Updateland(data);
            });
        }

        public LandBankFacadeModel.Bound GetLandMapBound()
        {
            PassContext(_service, _context);
            return _service.GetLandMapBound();
        }
    }
}
