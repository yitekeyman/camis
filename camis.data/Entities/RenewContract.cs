using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class RenewContract
{
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public Guid LandId { get; set; }

    public int? SplitIndex { get; set; }

    public int BudgetYear { get; set; }

    public long Date { get; set; }

    public string Remark { get; set; }

    public long? Aid { get; set; }

    public Guid Wfid { get; set; }

    public virtual Farm Farm { get; set; }

    public virtual Land Land { get; set; }

    public virtual ICollection<RenewContractDoc> RenewContractDocs { get; set; } = new List<RenewContractDoc>();
}
