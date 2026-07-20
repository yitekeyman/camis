using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities;

public partial class CancelledContractDoc
{
    public Guid Id { get; set; }

    public Guid CancellationId { get; set; }

    public Guid DocId { get; set; }

    public virtual CancelledContract Cancellation { get; set; }

    public virtual Document Doc { get; set; }
}
