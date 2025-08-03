using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Land
    {
        public Land()
        {
            AgroEchology = new HashSet<AgroEchology>();
            Irrigation = new HashSet<Irrigation>();
            LandAccessibility = new HashSet<LandAccessibility>();
            LandClimate = new HashSet<LandClimate>();
            LandDoc = new HashSet<LandDoc>();
            LandInvestment = new HashSet<LandInvestment>();
            LandMoisture = new HashSet<LandMoisture>();
            LandUpin = new HashSet<LandUpin>();
            LandUsage = new HashSet<LandUsage>();
            SoilTest = new HashSet<SoilTest>();
            Topography = new HashSet<Topography>();
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public int LandType { get; set; }
        public Guid Wid { get; set; }

        public AgriculturalZone AgriculturalZone { get; set; }
        public LandRight LandRight { get; set; }
        public ICollection<AgroEchology> AgroEchology { get; set; }
        public ICollection<Irrigation> Irrigation { get; set; }
        public ICollection<LandAccessibility> LandAccessibility { get; set; }
        public ICollection<LandClimate> LandClimate { get; set; }
        public ICollection<LandDoc> LandDoc { get; set; }
        public ICollection<LandInvestment> LandInvestment { get; set; }
        public ICollection<LandMoisture> LandMoisture { get; set; }
        public ICollection<LandUpin> LandUpin { get; set; }
        public ICollection<LandUsage> LandUsage { get; set; }
        public ICollection<SoilTest> SoilTest { get; set; }
        public ICollection<Topography> Topography { get; set; }
    }
}
