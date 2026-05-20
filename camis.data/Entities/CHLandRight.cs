using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHLandRight
{
    public Guid Id { get; set; }

    public int Seq { get; set; }

    public Guid ArchiveId { get; set; }

    public Guid FarmId { get; set; }

    public Guid LandId { get; set; }

    public int? LandSplitId { get; set; }

    public Guid? CommonTxtUid { get; set; }

    public string LandRightAttr { get; set; }

    public virtual CHArchive Archive { get; set; }
}
