using FileUploadDemo.Helpers;
using FileUploadDemo.Models;
using FileUploadDemo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FileUploadDemo.Controllers;

public class FilesController : ODataController
{
    private readonly IFileRepository _repo;

    public FilesController(IFileRepository repo)
    {
        _repo = repo;
    }

    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_repo.GetAll());
    }

    [EnableQuery]
    [HttpGet]
    public SingleResult<FileModel> Get([FromODataUri] Guid key)
    {
        return SingleResult.Create(_repo.GetById(key));
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        var createdFile = await ProcessAndSaveFile(file, new FileModel());

        if (createdFile == null) return BadRequest("Error creating file.");

        return CreatedAtAction(nameof(Get), new { id = createdFile.ID }, createdFile);
    }

    [HttpPost("odata/Files/" + nameof(CustomUpload))]
    public async Task<IActionResult> CustomUpload()
    {
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            return BadRequest("Invalid request: Not multipart content.");
        }

        var boundary = MultipartRequestHelper.GetBoundary(
            MediaTypeHeaderValue.Parse(Request.ContentType),
            lengthLimit: 200000); // Set an appropriate length limit for your needs

        var reader = new MultipartReader(boundary, HttpContext.Request.Body);

        var filesProcessed = new List<FileModel>(); // To keep track of processed files
        var section = await reader.ReadNextSectionAsync();
        while (section != null)
        {
            var hasContentDisposition = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

            if (hasContentDisposition)
            {
                if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                {
                    var file = new FileModel();
                    using var memoryStream = new MemoryStream();
                    await section.Body.CopyToAsync(memoryStream);
                    file.Data = memoryStream.ToArray();
                    file.MimeType = section.ContentType;
                    file.ID = Guid.NewGuid();
                    file.CreatedDate = DateTime.UtcNow;

                    var createdFile = _repo.Create(file); // Add the file to the database
                    filesProcessed.Add(createdFile); // Add to the list of processed files
                }
                else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                {
                    // Read and process other form data here
                    // For example, reading text fields from the form
                }
            }

            section = await reader.ReadNextSectionAsync();
        }

        if (filesProcessed.Any())
        {
            // Return a list of processed files, or some kind of summary
            return Ok(filesProcessed);
        }
        else
        {
            // No files were processed
            return BadRequest("No files were uploaded.");
        }
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromODataUri] Guid key, IFormFile file)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var dbFile = _repo.GetById(key).SingleOrDefault();
        if (dbFile == null) return NotFound($"File with ID {key} not found.");

        var updatedFile = await ProcessAndSaveFile(file, dbFile);

        if (updatedFile == null) return BadRequest("Error updating file.");

        return NoContent();
    }

    [HttpDelete]
    public IActionResult Delete([FromODataUri] Guid key)
    {
        var company = _repo.GetById(key);

        if (company is null) return BadRequest();

        _repo.Delete(company.First());
        return NoContent();
    }

    private async Task<FileModel> ProcessAndSaveFile(IFormFile file, FileModel fileModel)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        fileModel.Data = ms.ToArray();
        fileModel.MimeType = file.ContentType;

        if (fileModel.ID == Guid.Empty)
        {
            fileModel.ID = Guid.NewGuid();
            fileModel.CreatedDate = DateTime.UtcNow;
            // Assuming Create is a method that adds a new fileModel to the DB and saves the changes
            return _repo.Create(fileModel);
        }
        else
        {
            fileModel.LastModifiedDate = DateTime.UtcNow;
            // Assuming Update is a method that updates an existing fileModel in the DB and saves the changes
            _repo.Update(fileModel);
            return fileModel;
        }
    }
}

