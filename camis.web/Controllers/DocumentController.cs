using System;
using System.IO;
using intapscamis.camis.domain.Documents;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    [Roles]
    public class DocumentController : BaseController
    {
        private IDocumentFacade _facade;
        
        public DocumentController(IDocumentFacade facade)
        {
            _facade = facade;
        }

        
        public IActionResult GetAllDocumentTypes()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Ok(_facade.GetAllDocumentTypes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        
        public IActionResult DocumentFile(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                var doc = _facade.GetDocument(id.ToGuid());
                return File(doc.File, doc.Mimetype, null); // the filename is null to support in-browser view
            }
            catch (Exception e)
            { 
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        
        public IActionResult GetDocument(Guid id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Ok(_facade.GetDocument(id));
            }   
            catch (Exception e)
            { 
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        
        public IActionResult GetDocument(string Ref)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Ok(_facade.GetDocumentResponsesByRef(Ref));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        public IActionResult UploadDocument(DocumentRequest document)
        {
            try
            {
                _facade.CreateDocument(document);
                return Ok("Successful");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        public IActionResult Form()
        {
            return View();
        }
    }
}
