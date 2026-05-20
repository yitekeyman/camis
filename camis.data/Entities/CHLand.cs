using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHLand
{
    public Guid Id { get; set; }

    public int Seq { get; set; }

    public Guid ArchiveId { get; set; }

    public Guid LandId { get; set; }

    public string Upid { get; set; }

    public string LandAttr { get; set; }

    public virtual CHArchive Archive { get; set; }
}
