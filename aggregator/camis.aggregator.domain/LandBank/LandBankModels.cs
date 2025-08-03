using intapscamis.camis.data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace camis.aggregator.domain.LandBank
{
    public class InitLandData
    {
        public AccessibiltyType[] AccessibiltyType { get; set; }
        public AgroType[] AgroType { get; set; }
        public InverstmentType[] InvestmentType { get; set; }
        public SoilTestTypes[] SoilTestType { get; set; }
        public SurfaceWaterType[] SurfaceWaterType { get; set; }
        public TopographyType[] TopographyType { get; set; }
        public UsageType[] UsageType { get; set; }
        public WaterSourceType[] WaterSourceType { get; set; }
        public MoistureSource[] MoistureSource { get; set; }
        public GroundWater[] GroundWaterType { get; set; }
    }
}
