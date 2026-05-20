using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHOperator
{
    public Guid Id { get; set; }

    public int Seq { get; set; }

    public Guid ArchiveId { get; set; }

    public Guid? OperatorId { get; set; }

    public string OperatorAttr { get; set; }

    public virtual CHArchive Archive { get; set; }
}
