using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHFarm
{
    public Guid Id { get; set; }

    public int Seq { get; set; }

    public Guid ArchiveId { get; set; }

    public Guid FarmId { get; set; }

    public string FarmAttr { get; set; }

    public virtual CHArchive Archive { get; set; }
}
