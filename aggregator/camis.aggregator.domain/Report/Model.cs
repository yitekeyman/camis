using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static intapscamis.camis.domain.LandBank.LandBankFacadeModel;

namespace camis.aggregator.domain.Report
{
    public class RegionConfigModel
    {
        public string regionid { get; set; }
        public string url { get; set; }
        public string username { get; set; }
        public string password { get; set; }

    }
}
