using Backend.Application.Common;
using Backend.Application.Interfaces;
using Backend.Application.Orchestrator;
using Backend.Application.UseCases;
using Backend.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Tests.Tests
{
    [TestFixture]
    public class AudioProcessingOrchestratorTests
    {
        private Mock<IFileValidationService> _fileService;
        private Mock<IAudioConversionService> _convertService;
        private Mock<IMetadataService> _metadataService;

        private AudioProcessingOrchestrator _orchestrator;

        [SetUp]
        public void Setup()
        {
            _fileService = new Mock<IFileValidationService>();
            _convertService = new Mock<IAudioConversionService>();
            _metadataService = new Mock<IMetadataService>();

            _orchestrator = new AudioProcessingOrchestrator(
                new ValidateFileUseCase(_fileService.Object),
                new ConvertAudioUseCase(_convertService.Object),
                new UpdateMetadataUseCase(_metadataService.Object)
            );
        }

        [Test]
        public async Task Should_Succeed_When_AllStepsPass()
        {
            _fileService.Setup(x => x.EnsureFileExists(It.IsAny<string>()))
                .Returns(true);

            _convertService.Setup(x => x.ConvertAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<string>.Success("file.m4a"));

            _metadataService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<SoundMetadata>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var input = new TaskMetadata
            {
                SoundPath = "file.mp3",
                Author = "a",
                Title = "t"
            };

            var result = await _orchestrator.ProcessAsync(input, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task Should_Fail_When_FileNotFound()
        {
            _fileService.Setup(x => x.EnsureFileExists(It.IsAny<string>()))
                .Returns(false);

            var input = new TaskMetadata
            {
                SoundPath = "file.mp3",
                Author = "a",
                Title = "t"
            };

            var result = await _orchestrator.ProcessAsync(input, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task Should_Fail_When_ConversionFails()
        {
            _fileService.Setup(x => x.EnsureFileExists(It.IsAny<string>()))
                .Returns(true);

            _convertService.Setup(x => x.ConvertAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<string>.Failure("ffmpeg failed"));

            var input = new TaskMetadata
            {
                SoundPath = "file.mp3",
                Author = "a",
                Title = "t"
            };

            var result = await _orchestrator.ProcessAsync(input, CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
        }
    }
}
