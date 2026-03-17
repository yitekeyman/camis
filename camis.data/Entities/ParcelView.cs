using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

public class ParcelView
{
    [Key]
    [StringLength(100)]
    public string Upid { get; set; } = string.Empty;

    public Geometry Geometry { get; set; }

    [StringLength(100)]
    public string Region { get; set; }

    [StringLength(100)]
    public string Kebele { get; set; }

    [StringLength(100)]
    public string Woreda { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal? Area { get; set; }

    // Add other properties from your view as needed
}