using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using camis.aggregator.data.Entities;
using camis.aggregator.domain.LandBank;
using camis.aggregator.web.Filter;
using Microsoft.AspNetCore.Mvc;

namespace camis.aggregator.web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LandController : BaseController
    {
        private ILandBankFacade _land;
        private aggregatorContext _context;
        public LandController(ILandBankFacade facde, aggregatorContext Context)
        {
            _land = facde;
            _context = Context;
           
        }

        [Roles]
        [HttpGet]
        public IActionResult GetLandList()
        {
            try
            {
                _land.SetSession(GetSession());
                var response = _land.GetLandList();
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }


        [Roles]
        [HttpGet]
        public IActionResult GetInitData()
        {
            try
            {
                _land.SetSession(GetSession());
                var response = _land.GetInitData();
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Roles]
        [HttpGet]
        public IActionResult GetLand(Guid id)
        {
            try
            {
                _land.SetSession(GetSession());
                var response = _land.GetLand(id, true, true);
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Roles]
        [HttpPost]
        public IActionResult SynchronizeLand([FromBody] string[] regions)
        {
            try
            {
                _land.SetSession(GetSession());
                var response = _land.SynchronizeLand(regions);
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Roles]
        [HttpGet]
        public IActionResult GetLandListOfRegion(string region)
        {
            try
            {
                _land.SetSession(GetSession());
                var response = _land.GetLandList(region);
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

    }
}