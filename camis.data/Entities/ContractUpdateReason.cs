using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class ContractUpdateReason
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<CHArchive> CHArchives { get; set; } = new List<CHArchive>();
}
