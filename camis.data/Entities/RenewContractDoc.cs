using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class RenewContractDoc
{
    public Guid Id { get; set; }

    public Guid RenewId { get; set; }

    public Guid DocId { get; set; }

    public int? TypeId { get; set; }

    public int? AuthorityId { get; set; }

    public virtual Document Doc { get; set; }

    public virtual RenewContract Renew { get; set; }
}
