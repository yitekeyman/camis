using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.Workflows;
using Microsoft.Extensions.Logging.Abstractions;

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
        Document ParseDocumentFromFolder(Guid workItem, DocumentRequest data);
        Document CreateDocumentInFolder(Guid workItem, DocumentRequest data);
        Guid ExtractWorkItemIdFromOverrideFilePath(string overrideFilePath);
        Document GetDocumentFileFromFolder(Guid id);
        void DeleteDocumentFromFolder(Guid id);
        public void DeleteFileFromFolder(IList<Document> documents);
        Document UpdateDocumentFromFolder(Guid id, DocumentRequest data);
        void PatchSaveDocument();
        Task<int> PatchMigratingWorkItemFile();
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
        
        public Document CreateDocumentInFolder(Guid workItem, DocumentRequest data)
        {
            var doc = ParseDocumentFromFolder(workItem, data);
            var doc2 = SaveDocumentAtFolder(doc);
    
            // Check if document already exists
            var existingDoc = Context.Document.Find(doc2.Id);
            if (existingDoc != null)
            {
                // Update existing document
                existingDoc.Date = doc2.Date;
                existingDoc.Ref = doc2.Ref;
                existingDoc.Note = doc2.Note;
                existingDoc.Mimetype = doc2.Mimetype;
                existingDoc.Type = doc2.Type;
                existingDoc.Filename = doc2.Filename;
                existingDoc.OverrideFilePath = doc2.OverrideFilePath;
                // Don't set File property - keep it null
                existingDoc.File = null;
        
                Context.Document.Update(existingDoc);
                Context.SaveChanges(_session.Username, (int)UserActionType.UpdateDocument);
                return existingDoc;
            }
            else
            {
                Context.Document.Add(doc2);
                Context.SaveChanges();
                doc2.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.CreateDocument).Id;
                Context.Update(doc2);
                return doc2;
            }
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
        public Document UpdateDocumentFromFolder(Guid id, DocumentRequest data)
        {
            var existingDoc = GetDocument(id);
            if (existingDoc == null) return null;
    
            // Update properties
            existingDoc.Date = data.Date;
            existingDoc.Ref = data.Ref;
            existingDoc.Note = data.Note;
            existingDoc.Mimetype = data.Mimetype;
            existingDoc.Type = data.Type;
            existingDoc.Filename = data.Filename;
    
            // Save to folder and clear the File property
            var updatedDoc = SaveDocumentAtFolder(existingDoc);
    
            Context.Document.Update(updatedDoc);
            updatedDoc.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.UpdateDocument).Id;
            Context.Update(updatedDoc);
    
            return updatedDoc;
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
        public static DocumentResponse ParseOperatorPhotoResponse(Document doc)
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
                    File = doc.File
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
                    File = data.File==null?null: Convert.FromBase64String(data.File),
                    OverrideFilePath = data.OverrideFilePath
                };
        }
        public Document ParseDocumentFromFolder(Guid workItem, DocumentRequest data)
        {
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "C:\\usr\\bin\\CAMIS\\data\\docs";
            
            var filePath = $"{data.Id}";
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                    workItem.ToString(), Path.GetFileName(filePath));
            }
            if (File.Exists(filePath))
            {
                var fileBytes = File.ReadAllBytes(filePath);
                data.File = Convert.ToBase64String(fileBytes);
                
            }
            
            return ParseDocument(data);
        }

        private Document SaveDocumentAtFolder(Document data)
        {
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "C:\\usr\\bin\\CAMIS\\data\\docs";
            
           
            var filePath2 = $"{data.Id}";
            const string pathPrefix = "/api/Document/GetDocumentFileFromFolder";
            filePath2 = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory, Path.GetFileName(filePath2));
            if (File.Exists(filePath2))
            {
                File.Delete(filePath2);
            }
            File.WriteAllBytes(filePath2, data.File);
            data.OverrideFilePath=$"{pathPrefix}?docId={data.Id}";
            data.File = null;
            return data;
        }
        public Guid ExtractWorkItemIdFromOverrideFilePath(string overrideFilePath)
        {
            if (string.IsNullOrEmpty(overrideFilePath))
                return Guid.Empty;
    
            // Parse as URI
            if (Uri.TryCreate(overrideFilePath, UriKind.RelativeOrAbsolute, out var uri))
            {
                // Get the last segment of the path
                var segments = uri.OriginalString.Split('/');
                var lastSegment = segments[segments.Length - 1];
        
                // Remove query parameters if present
                var queryIndex = lastSegment.IndexOf('?');
                if (queryIndex > 0)
                {
                    lastSegment = lastSegment.Substring(0, queryIndex);
                }
        
                // Parse to Guid
                if (Guid.TryParse(lastSegment, out var workItemId))
                {
                    return workItemId;
                }
            }
    
            return Guid.Empty;
        }
        public Document GetDocumentFileFromFolder(Guid id)
        {

            Document ret = null;
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "C:\\usr\\bin\\CAMIS\\data\\docs";
            var doc= GetDocument(id);
            if (doc != null)
            {
                var filePath = $"{doc.Id.ToString()}";
                if (!Path.IsPathRooted(filePath))
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory, Path.GetFileName(filePath));
                }
                if (File.Exists(filePath))
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    doc.File = fileBytes;
                
                }
                ret = doc;
            }
           
            
            return ret;
        }

        public void DeleteDocumentFromFolder(Guid id)
        {
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "C:\\usr\\bin\\CAMIS\\data\\docs";
            var filePath = $"{id.ToString()}";
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory, Path.GetFileName(filePath));
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        public void DeleteFileFromFolder(IList<Document> documents)
        {
            foreach (var doc in documents)
            {
                DeleteDocumentFromFolder(doc.Id);
            }
        }

        public void PatchSaveDocument()
        {
            var docList = Context.Document.Where(d=>d.File!=null).ToList();
            var docLists=new List<Document>();
            foreach (var doc in docList)
            {
                docLists.Add( SaveDocumentAtFolder(doc));
            }
            Context.Document.UpdateRange(docLists);
            Context.SaveChanges();
        }

        public async Task<int> PatchMigratingWorkItemFile()
        {
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "C:\\usr\\bin\\CAMIS\\data\\docs";
            var total = Context.WorkItem.Count();
            var pageSize = 100;
            var processor = new WorkItemFileProcessor(fileDirectory);
    
            // Fix: Calculate pages correctly
            var pages = (int)Math.Ceiling((double)total / pageSize);
            var workItems = 0;
    
            for (int i = 0; i < pages; i++)
            {
                var skip = i * pageSize;
                var take = pageSize;  // Fix: This should be pageSize, not skip + pageSize
                workItems += await processor.ProcessWorkItemFilesAsync(skip, take);
            }
    
            return workItems;

        }
    }
}
