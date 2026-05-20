using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHActivity
{
    public Guid Id { get; set; }

    public int Seq { get; set; }

    public Guid ArchiveId { get; set; }

    public Guid ActivityId { get; set; }

    public string ActivityAttr { get; set; }

    public virtual CHArchive Archive { get; set; }
}
