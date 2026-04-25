using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class FarmStatusType
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();
}
