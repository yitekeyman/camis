using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHLandSplit
{
    public Guid Id { get; set; }

    public int Seq { get; set; }

    public Guid ArchiveId { get; set; }

    public Guid LandId { get; set; }

    public int SplitId { get; set; }

    public string LandSplitAttr { get; set; }

    public virtual CHArchive Archive { get; set; }
}
