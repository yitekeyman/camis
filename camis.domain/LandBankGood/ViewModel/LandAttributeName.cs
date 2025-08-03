using System;
using System.Collections.Generic;
using System.Text;

namespace intapscamis.camis.domain.LandBankGood.ViewModel
{
    public class LandAttributeName
    {
        public List<Type> AgroType { get; set; } = new List<Type>();
        public List<Type> InvestmentType { get; set; } = new List<Type>();
        public List<Type> WaterSourceType { get; set; } = new List<Type>();
        public List<Type> SurfaceWaterType { get; set; } = new List<Type>();
        public List<Type> TopographyType { get; set; } = new List<Type>();
        public List<Type> LandUsageType { get; set; } = new List<Type>();
    }
    public class Type
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
