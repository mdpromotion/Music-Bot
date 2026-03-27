using Backend.Features.FileUpload.Services;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Backend.Tests.FileUpload
{
    public class FileStorageServiceTests
    {
        private string _tempDir = null!;
        private FileStorageService _service = null!;

        [SetUp]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _service = new FileStorageService(_tempDir);
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        [Test]
        public async Task SaveAndGetFileAsync_WorksForAudioFile()
        {
            var content = Encoding.UTF8.GetBytes("fake audio content");
            var stream = new MemoryStream(content);
            var file = new FormFile(stream, 0, content.Length, "file", "audio.mp3");

            var fileId = await _service.SaveFileAsync(file);
            var storedFile = await _service.GetFileAsync(fileId);

            Assert.IsNotNull(storedFile);
            Assert.AreEqual(fileId, storedFile!.FileName);
            Assert.AreEqual(content, storedFile.Content);
        }
    }
}