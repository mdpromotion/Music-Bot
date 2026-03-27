using Backend.Features.FileUpload.Controllers;
using Backend.Features.FileUpload.Models;
using Backend.Features.FileUpload.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace Backend.Tests.FileUpload
{
    public class FileUploadControllerTests
    {
        private Mock<IFileStorageService> _fileStorageMock = null!;
        private FileUploadController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _fileStorageMock = new Mock<IFileStorageService>();
            _controller = new FileUploadController(_fileStorageMock.Object);
        }

        [Test]
        public async Task UploadFile_NullFile_ReturnsBadRequest()
        {
            var result = await _controller.UploadFile(null!);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UploadFile_ValidAudioFile_ReturnsFileId()
        {
            var content = Encoding.UTF8.GetBytes("fake audio content");
            var stream = new MemoryStream(content);
            var formFile = new FormFile(stream, 0, content.Length, "file", "test.mp3");

            _fileStorageMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                            .ReturnsAsync("file-id-123");

            var result = await _controller.UploadFile(formFile) as OkObjectResult;

            Assert.IsNotNull(result);

            var value = result!.Value as UploadFileResponse;
            Assert.IsNotNull(value);
            Assert.AreEqual("file-id-123", value.FileId);
        }

        [Test]
        public async Task DownloadFile_FileNotFound_ReturnsNotFound()
        {
            _fileStorageMock.Setup(x => x.GetFileAsync("file-id")).ReturnsAsync((StoredFile?)null);

            var result = await _controller.DownloadFile("file-id");

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DownloadFile_ExistingFile_ReturnsFile()
        {
            var storedFile = new StoredFile
            {
                FileName = "test.mp3",
                Content = Encoding.UTF8.GetBytes("audio content"),
                ContentType = "audio/mpeg"
            };

            _fileStorageMock.Setup(x => x.GetFileAsync("file-id")).ReturnsAsync(storedFile);

            var result = await _controller.DownloadFile("file-id") as FileContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(storedFile.ContentType, result!.ContentType);
            Assert.AreEqual(storedFile.Content, result.FileContents);
        }
    }
}