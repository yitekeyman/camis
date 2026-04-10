using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Documents.Models;

namespace intapscamis.camis.domain.Documents
{
    public interface IDocumentFacade : ICamisFacade
    {
        void SetSession(UserSession session);

        List<DocumentType> GetAllDocumentTypes();
        List<DocumentResponse> GetDocumentResponsesByRef(string Ref);

        DocumentResponse GetDocumentResponse(Guid id);
        Document GetDocument(Guid id);

        Document CreateDocument(DocumentRequest data);
        Document UpdateDocument(Guid id, DocumentRequest data);
        Document DeleteDocument(Guid id);
        Document ParseDocumentFromFolder(Guid workItem, DocumentRequest data);
        Document CreateDocumentInFolder(Guid workItem, DocumentRequest data);
        Guid ExtractWorkItemIdFromOverrideFilePath(string overrideFilePath);
        Document GetDocumentFileFromFolder(Guid id);
        void PatchSaveDocument();
        Task<int> PatchMigratingWorkItemFile();
    }

    public class DocumentFacade : CamisFacade, IDocumentFacade
    {
        private readonly IDocumentService _service;
        private UserSession _session;

        private readonly CamisContext _context;

        public DocumentFacade(CamisContext context, IDocumentService service)
        {
            _context = context;
            _service = service;
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _service.SetSession(_session);
        }


        public List<DocumentType> GetAllDocumentTypes()
        {
            PassContext(_service, _context);
            return _service.GetAllDocumentTypes();
        }

        public List<DocumentResponse> GetDocumentResponsesByRef(string Ref)
        {
            PassContext(_service, _context);
            return _service.GetDocumentResponsesByRef(Ref);
        }


        public DocumentResponse GetDocumentResponse(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetDocumentResponse(id);
        }

        public Document GetDocument(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetDocument(id);
        }


        public Document CreateDocument(DocumentRequest data)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.CreateDocument(data);
            });
        }

        public Document UpdateDocument(Guid id, DocumentRequest data)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.UpdateDocument(id, data);
            });
        }

        public Document DeleteDocument(Guid id)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.DeleteDocument(id);
            });
        }

        public Document ParseDocumentFromFolder(Guid workItem, DocumentRequest data)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.ParseDocumentFromFolder(workItem, data);
            });
        }

        public Document CreateDocumentInFolder(Guid workItem, DocumentRequest data)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.CreateDocumentInFolder(workItem, data);
            });
        }

        public Guid ExtractWorkItemIdFromOverrideFilePath(string overrideFilePath)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.ExtractWorkItemIdFromOverrideFilePath(overrideFilePath);
            });
        }

        public Document GetDocumentFileFromFolder(Guid id)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.GetDocumentFileFromFolder(id);
            });
        }

        public void PatchSaveDocument()
        {
            PassContext(_service, _context);
            _service.PatchSaveDocument();
        }

        public async Task<int> PatchMigratingWorkItemFile()
        { 
            PassContext(_service, _context);
            return await _service.PatchMigratingWorkItemFile();
        }
    }
}