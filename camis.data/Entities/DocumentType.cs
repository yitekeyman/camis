using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class DocumentType
    {
        public DocumentType()
        {
            Document = new HashSet<Document>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Document> Document { get; set; }
    }
}
