
using intapscamis.camis.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace intapscamis.camis.domain.Workflows;

public class WorkItemFileProcessor
{
    private readonly CamisContext _context;
    private readonly string _fileDirectory;
    private readonly ILogger _logger;

    public WorkItemFileProcessor( string fileDirectory, ILogger logger = null)
    {
        _context = new CamisContext();
        _fileDirectory = fileDirectory;
        _logger = logger;
    }

  
    /// Process work items to extract bytea files from JSON data, save to disk, and set to null
 
    public async Task<int> ProcessWorkItemFilesAsync(int skip, int take)
    {
        

        var workItems = _context.WorkItem.OrderBy(i => i.Id).Skip(skip).Take(take).ToList();
        var processedCount = 0;
        var wis = new List<WorkItem>();
        foreach (var workItem in workItems)
        {
            if (await ProcessWorkItemAsync(workItem))
            {
                processedCount++;
                wis.Add(workItem);
            }
        }
        _context.WorkItem.UpdateRange(wis);
        await _context.SaveChangesAsync();
        return processedCount;
    }

  
    /// Process a single work item
    private async Task<bool> ProcessWorkItemAsync(WorkItem workItem)
    {
        if (string.IsNullOrEmpty(workItem.Data))
        {
            _logger?.LogDebug($"WorkItem {workItem.Id} has no data");
            return false;
        }

        try
        {
            var originalData = workItem.Data; // Store original for comparison
            var jsonObject = JObject.Parse(workItem.Data);
            var modified = false;

            // Process different data types
            modified = await ProcessFarmRequestAsync(workItem, jsonObject) || modified;
            modified = await ProcessActivityPlanRequestAsync(workItem, jsonObject) || modified;
            modified = await ProcessDocumentRequestAsync(workItem, jsonObject) || modified;

            if (modified)
            {
                var newData = jsonObject.ToString(Formatting.None);
                if (originalData != newData) // Only update if actually changed
                {
                    workItem.Data = newData;
                    _logger?.LogInformation($"Processed work item {workItem.Id} of type {workItem.DataType}");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error processing work item {workItem.Id}");
        }

        return false;
    }


    /// Process FarmRequest type data
  
    private async Task<bool> ProcessFarmRequestAsync(WorkItem workItem, JObject jsonObject)
    {
        var modified = false;

        // Process Operator Photo
        var photoToken = jsonObject.SelectToken("$.Operator.Photo.File");
        if (photoToken?.Type == JTokenType.String && !string.IsNullOrEmpty(photoToken.Value<string>()))
        {
            var photoId = jsonObject.SelectToken("$.Operator.Photo.Id")?.Value<Guid>();
            if (photoId.HasValue)
            {
                var  pathPrefix = "/api/Farms/InWorkItemOperatorPhoto/";
                await SaveByteaToFileAsync(workItem.Id, photoId.Value, photoToken.Value<string>());
                jsonObject.SelectToken("$.Operator.Photo.File")?.Replace(null);
                if (string.IsNullOrEmpty(jsonObject.SelectToken("$.Operator.Photo.OverrideFilePath")?.Value<string>()))
                {
                   pathPrefix= $"{pathPrefix}{workItem.Id}?photoId={photoId.Value}";
                   jsonObject.SelectToken("$.Operator.Photo.OverrideFilePath")?.Replace(pathPrefix);
                }
                modified = true;
                _logger?.LogDebug($"Removed photo file from work item {workItem.Id}");
            }
        }

        // Process Operator Registrations Documents
        var opRegistrations = jsonObject.SelectTokens("$.Operator.Registrations[*]");
        foreach (var fileToken in opRegistrations.ToList())
        {
            
            if (fileToken.Type == JTokenType.Object)
            {
                var  pathPrefix = "/api/Farms/InWorkItemOperatorRegistrationFile/";
                var docIdToken = fileToken.SelectToken("$.Document.Id");
                var docFileToken=fileToken.SelectToken("$.Document.File");
                if (docIdToken?.Type == JTokenType.String)
                {
                    var docId = Guid.Parse(docIdToken.Value<string>());
                    await SaveByteaToFileAsync(workItem.Id, docId, docFileToken.Value<string>(), "registration");
                    if (string.IsNullOrEmpty(fileToken.SelectToken("$.Document.OverrideFilePath")?.Value<string>()))
                    {
                        var regIdToken=fileToken.SelectToken("$.Id");
                        pathPrefix = $"{pathPrefix}{workItem.Id}?regId={regIdToken?.Value<int>()}";
                        fileToken.SelectToken("$.Document.OverrideFilePath")?.Replace(pathPrefix);
                        
                    }
                    fileToken.SelectToken("$.Document.File")?.Replace(null);
                    modified = true;
                }
                
            }
        }

        // Process Farm Registrations Documents
        var farmRegistrations = jsonObject.SelectTokens("$.Registrations[*]");
        foreach (var fileToken in farmRegistrations.ToList())
        {
            if (fileToken.Type == JTokenType.Object)
            {
                var  pathPrefix = "/api/Farms/InWorkItemRegistrationFile/";
                var docIdToken = fileToken.SelectToken("$.Document.Id");
                var docFileToken=fileToken.SelectToken("$.Document.File");
                if (docIdToken?.Type == JTokenType.String)
                {
                    var docId = Guid.Parse(docIdToken.Value<string>());
                    await SaveByteaToFileAsync(workItem.Id, docId, docFileToken.Value<string>(), "farm_registration");
                    if (string.IsNullOrEmpty(fileToken.SelectToken("$.Document.OverrideFilePath")?.Value<string>()))
                    {
                        var regIdToken=fileToken.SelectToken("$.Id");
                        pathPrefix = $"{pathPrefix}{workItem.Id}?regId={regIdToken?.Value<int>()}";
                        fileToken.SelectToken("$.Document.OverrideFilePath")?.Replace(pathPrefix);
                        
                    }
                    fileToken.SelectToken("$.Document.File")?.Replace(null);
                    modified = true;
                }

               
            }
        }

        // Process Activity Plan Documents
        var activityDocs = jsonObject.SelectTokens("$.ActivityPlan.Documents[*]");
        foreach (var fileToken in activityDocs.ToList())
        {
            if (fileToken.Type == JTokenType.Object)
            {
                var  pathPrefix = "/api/Farms/InWorkItemActivityPlanFile/";
                var docIdToken = fileToken.SelectToken("$.Id");
                var docFileToken=fileToken.SelectToken("$.File");
                if (docIdToken?.Type == JTokenType.String)
                {
                    var docId = Guid.Parse(docIdToken.Value<string>());
                    await SaveByteaToFileAsync(workItem.Id, docId, docFileToken.Value<string>(), "farm_registration");
                    if (string.IsNullOrEmpty(fileToken.SelectToken("$.OverrideFilePath")?.Value<string>()))
                    {
                    
                        pathPrefix = $"{pathPrefix}{workItem.Id}?documentId={docId}";
                        fileToken.SelectToken("$.OverrideFilePath")?.Replace(pathPrefix);
                        
                    }
                    fileToken.SelectToken("$.File")?.Replace(null);
                    modified = true;
                }

               
            }
        }

        return modified;
    }
    
    /// Process ActivityPlanRequest type data
    private async Task<bool> ProcessActivityPlanRequestAsync(WorkItem workItem, JObject jsonObject)
    {
        var modified = false;
        
        var documents = jsonObject.SelectTokens("$.Documents[*]");
        foreach (var fileToken in documents.ToList())
        {
            if (fileToken.Type == JTokenType.Object)
            {
                var  pathPrefix = "/api/Farms/InWorkItemActivityPlanFileForPlanUpdate/";
                var docIdToken = fileToken.SelectToken("$.Id");
                var docFileToken=fileToken.SelectToken("$.File");
                if (docIdToken?.Type == JTokenType.String)
                {
                    var docId = Guid.Parse(docIdToken.Value<string>());
                    await SaveByteaToFileAsync(workItem.Id, docId, docFileToken.Value<string>(), "farm_registration");
                    if (string.IsNullOrEmpty(fileToken.SelectToken("$.OverrideFilePath")?.Value<string>()))
                    {
                    
                        pathPrefix = $"{pathPrefix}{workItem.Id}?documentId={docId}";
                        fileToken.SelectToken("$.OverrideFilePath")?.Replace(pathPrefix);
                        
                    }
                    fileToken.SelectToken("$.File")?.Replace(null);
                    modified = true;
                }

               
            }
        }
        
        var reportDocuments = jsonObject.SelectTokens("$.ReportDocuments[*]");
        foreach (var fileToken in reportDocuments.ToList())
        {
            if (fileToken.Type == JTokenType.Object)
            {
                var  pathPrefix = "/api/Farms/InWorkItemReportFile/";
                var docIdToken = fileToken.SelectToken("$.Id");
                var docFileToken=fileToken.SelectToken("$.File");
                if (docIdToken?.Type == JTokenType.String)
                {
                    var docId = Guid.Parse(docIdToken.Value<string>());
                    await SaveByteaToFileAsync(workItem.Id, docId, docFileToken.Value<string>(), "farm_registration");
                    if (string.IsNullOrEmpty(fileToken.SelectToken("$.OverrideFilePath")?.Value<string>()))
                    {
                    
                        pathPrefix = $"{pathPrefix}{workItem.Id}?documentId={docId}";
                        fileToken.SelectToken("$.OverrideFilePath")?.Replace(pathPrefix);
                        
                    }
                    fileToken.SelectToken("$.File")?.Replace(null);
                    modified = true;
                }

               
            }
        }

        return modified;
    }

  
    /// Process generic DocumentRequest type data
   
    private async Task<bool> ProcessDocumentRequestAsync(WorkItem workItem, JObject jsonObject)
    {
        var modified = false;
        
        var fileToken = jsonObject.SelectToken("$.File");
        if (fileToken?.Type == JTokenType.String && !string.IsNullOrEmpty(fileToken.Value<string>()))
        {
            var idToken = jsonObject.SelectToken("$.Id");
            if (idToken?.Type == JTokenType.String)
            {
                var docId = Guid.Parse(idToken.Value<string>());
                await SaveByteaToFileAsync(workItem.Id, docId, fileToken.Value<string>(), "document");
            }
            fileToken.Replace(null);
            modified = true;
        }

        return modified;
    }


    /// Save bytea data to file system
   
    private async Task SaveByteaToFileAsync(Guid workItemId, object documentId, string base64Data, string prefix = "doc")
    {
        try
        {
            // Create directory if not exists
            var workItemFolder = Path.Combine(_fileDirectory, workItemId.ToString());
            if (!Directory.Exists(workItemFolder))
            {
                Directory.CreateDirectory(workItemFolder);
            }

            // Generate filename
            var fileName = $"{documentId}";
            var filePath = Path.Combine(workItemFolder, fileName);

            // Convert base64 to bytes and save
            var fileBytes = Convert.FromBase64String(base64Data);
            if(File.Exists(filePath))
                File.Delete(filePath);
            await File.WriteAllBytesAsync(filePath, fileBytes);

            _logger?.LogDebug($"Saved file to {filePath} (Size: {fileBytes.Length} bytes)");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Failed to save file for work item {workItemId}, document {documentId}");
            throw;
        }
    }

   

}