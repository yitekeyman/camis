using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.data.Entities;

public partial class EnvironmentalChangeEvent
{
    public int Id { get; set; }

    public string ParcelUpid { get; set; }

    public DateTime EventDate { get; set; }

    public string EventType { get; set; }

    public string EventSubtype { get; set; }

    public decimal? BeforeValue { get; set; }

    public decimal? AfterValue { get; set; }

    public decimal? ChangeAmount { get; set; }

    public decimal? ChangePercentage { get; set; }

    public decimal? AffectedAreaSqkm { get; set; }

    public string Severity { get; set; }

    public decimal? Confidence { get; set; }

    public Geometry Geometry { get; set; }

    public Geometry Centroid { get; set; }

    public string Description { get; set; }

    public bool? SatelliteEvidence { get; set; }

    public bool? Verified { get; set; }

    public DateTime? DetectedAt { get; set; }
}
