using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using intapscamis.camis.domain.Documents.Models;

namespace intapscamis.camis.domain.Documents
{
    public interface IDocumentService : ICamisService
    {
        void SetSession(UserSession session);

        List<DocumentType> GetAllDocumentTypes();
        List<DocumentResponse> GetDocumentResponsesByRef(string Ref);

        DocumentResponse GetDocumentResponse(Guid id);
        Document GetDocument(Guid? id);

        Document CreateDocument(DocumentRequest data);
        Document UpdateDocument(Guid id, DocumentRequest data);
        Document DeleteDocument(Guid id);
    }

    public class DocumentService : CamisService, IDocumentService
    {
        private UserSession _session;

        public void SetSession(UserSession session)
        {
            _session = session;
        }


        public List<DocumentType> GetAllDocumentTypes()
        {
            var docs = Context.DocumentType.ToList();
            return docs;
        }

        public List<DocumentResponse> GetDocumentResponsesByRef(string Ref)
        {
            var docs = Context.Document.Where(m => m.Ref == Ref).ToList();
            return docs.Select(ParseDocumentResponse).ToList();
        }

        
        public Document GetDocument(Guid? id)
        {
            return id == null ? null : Context.Document.Find(id);
        }

        public DocumentResponse GetDocumentResponse(Guid id)
        {
            var doc = Context.Document.Find(id);
            return ParseDocumentResponse(doc);
        }


        public Document CreateDocument(DocumentRequest data)
        {
            var doc = ParseDocument(data);
            
            Context.Document.Add(doc);
            doc.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.CreateDocument).Id;
            Context.Update(doc);

            return doc;
        }

        public Document UpdateDocument(Guid id, DocumentRequest data)
        {
            var doc = GetDocument(id);

            doc.Date = data.Date;
            doc.Ref = data.Ref;
            doc.Note = data.Note;
            doc.Mimetype = data.Mimetype;
            doc.Type = data.Type;
            doc.Filename = data.Filename;
            if (data.File != null) doc.File = Convert.FromBase64String(data.File);
            doc.OverrideFilePath = data.OverrideFilePath;

            Context.Document.Update(doc);
            doc.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.UpdateDocument).Id;
            Context.Update(doc);

            return doc;
        }

        public Document DeleteDocument(Guid id)
        {
            var doc = GetDocument(id);
            
            Context.Document.Remove(doc);
            doc.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.DeleteDocument).Id;

            return doc;
        }

        
        public static DocumentResponse ParseDocumentResponse(Document doc)
        {
            return doc == null
                ? null
                : new DocumentResponse
                {
                    Id = doc.Id,
                    Date = doc.Date,
                    Filename = doc.Filename,
                    Mimetype = doc.Mimetype,
                    Note = doc.Note,
                    Ref = doc.Ref,
                    Type = doc.Type,
                    OverrideFilePath = doc.OverrideFilePath
                };
        }
        
        public static Document ParseDocument(DocumentRequest data)
        {
            return data == null
                ? null
                : new Document
                {
                    Id = data.Id ?? Guid.NewGuid(),
                    Date = data.Date,
                    Ref = data.Ref,
                    Note = data.Note,
                    Mimetype = data.Mimetype,
                    Type = data.Type,
                    Filename = data.Filename,
                    File = Convert.FromBase64String(data.File),
                    OverrideFilePath = data.OverrideFilePath
                };
        }
    }
}
