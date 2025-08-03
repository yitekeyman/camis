using System;

namespace intapscamis.camis.domain.Documents.Models
{
    public class DocumentRequest
    {
        public long Date { get; set; }
        public string Ref { get; set; }
        public string Note { get; set; }
        public string Mimetype { get; set; }
        public int? Type { get; set; }
        public string Filename { get; set; }
        public string File { get; set; }
      

        // only for viewing documents from a custom url (esp. in work items)
        public Guid? Id { get; set; }
        public string OverrideFilePath { get; set; }
    }
/*    public enum DocumentTypes
    {
        BusinessPlan=10
    }*/
}