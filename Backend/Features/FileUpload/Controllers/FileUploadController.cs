using Backend.Features.FileUpload.Models;
using Backend.Features.FileUpload.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.FileUpload.Controllers
{
    [Route("api/file-upload")]
    [ApiController]
    public class FileUploadController(IFileStorageService fileUploadService) : ControllerBase
    {
        private readonly IFileStorageService _fileUploadService = fileUploadService;

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileId = await _fileUploadService.SaveFileAsync(file);
            return Ok(new UploadFileResponse { FileId = fileId });
        }

        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var storedFile = await _fileUploadService.GetFileAsync(fileId);
            if (storedFile == null)
                return NotFound();

            return File(storedFile.Content, storedFile.ContentType, storedFile.FileName);
        }
    }
}