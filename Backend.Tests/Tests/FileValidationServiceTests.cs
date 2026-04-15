using Backend.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Tests.Tests
{
    [TestFixture]
    public class FileValidationServiceTests
    {
        private FileValidationService _service;

        [SetUp]
        public void Setup()
        {
            _service = new FileValidationService();
        }

        [Test]
        public void Should_Fail_WhenFileDoesNotExist()
        {
            var result = _service.EnsureFileExists("not_existing_file.mp3");

            Assert.IsFalse(result);
        }

        [Test]
        public void Should_Succeed_WhenFileExists()
        {
            var tempFile = Path.GetTempFileName();

            var result = _service.EnsureFileExists(tempFile);

            Assert.IsTrue(result);

            File.Delete(tempFile);
        }
    }
}
