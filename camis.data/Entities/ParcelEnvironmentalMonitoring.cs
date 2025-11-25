using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.data.Entities;

public partial class ParcelEnvironmentalMonitoring
{
    public int Id { get; set; }

    public string ParcelUpid { get; set; }

    public DateTime MonitoringDate { get; set; }

    /// <summary>
    /// Vegetation index
    /// </summary>
    public decimal? NDVI { get; set; }

    /// <summary>
    /// Water index 
    /// </summary>
    public decimal? NDWI { get; set; }

    /// <summary>
    /// Built-up index
    /// </summary>
    public decimal? NDBI { get; set; }

    /// <summary>
    /// Enhanced vegetation index
    /// </summary>
    public decimal? EVI { get; set; }

    /// <summary>
    /// Modified water index
    /// </summary>
    public decimal? MNDWI { get; set; }

    /// <summary>
    /// VEGETATION_LOSS, VEGETATION_GROWTH, WATER_CHANGE, URBANIZATION, FLOOD, DROUGHT
    /// </summary>
    public string ChangeType { get; set; }

    public decimal? ChangeMagnitude { get; set; }

    public decimal? Confidence { get; set; }

    public string Severity { get; set; }

    public decimal? SoilMoisture { get; set; }

    public decimal? VegetationHealth { get; set; }

    public decimal? WaterPresence { get; set; }

    public Geometry Geometry { get; set; }

    public decimal? AreaSqkm { get; set; }

    public string SatelliteSource { get; set; }

    public decimal? CloudCover { get; set; }

    public DateTime? CreatedAt { get; set; }
}
