using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CHArchive
{
    public Guid Id { get; set; }

    public int TypeId { get; set; }

    public int UpdateReasonId { get; set; }

    public string Description { get; set; }

    public long RequestedOn { get; set; }

    public string RequestedBy { get; set; }

    public long? ApprovedOn { get; set; }

    public string ApprovedBy { get; set; }

    public Guid Wfid { get; set; }

    public int? Aid { get; set; }

    public string Data { get; set; }

    public virtual User ApprovedByNavigation { get; set; }

    public virtual ICollection<CHActivity> CHActivities { get; set; } = new List<CHActivity>();

    public virtual ICollection<CHFarmLand> CHFarmLands { get; set; } = new List<CHFarmLand>();

    public virtual ICollection<CHFarm> CHFarms { get; set; } = new List<CHFarm>();

    public virtual ICollection<CHLandRight> CHLandRights { get; set; } = new List<CHLandRight>();

    public virtual ICollection<CHLandSplit> CHLandSplits { get; set; } = new List<CHLandSplit>();

    public virtual ICollection<CHLand> CHLands { get; set; } = new List<CHLand>();

    public virtual ICollection<CHOperator> CHOperators { get; set; } = new List<CHOperator>();

    public virtual User RequestedByNavigation { get; set; }

    public virtual WorkflowType Type { get; set; }

    public virtual ContractUpdateReason UpdateReason { get; set; }

    public virtual Workflow Wf { get; set; }
}
