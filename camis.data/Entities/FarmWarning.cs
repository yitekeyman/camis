using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class FarmWarning
{
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid LandId { get; set; }

    public int SplitIndex { get; set; }

    public long Date { get; set; }

    public int Stage { get; set; }

    public string Reason { get; set; }

    public string ReasonDetails { get; set; }

    public long? Aid { get; set; }

    public Guid? Wfid { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual Land Land { get; set; }

    public virtual ICollection<WarningDoc> WarningDocs { get; set; } = new List<WarningDoc>();
}
