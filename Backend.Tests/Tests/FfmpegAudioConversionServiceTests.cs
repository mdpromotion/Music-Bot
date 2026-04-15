using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Services;
using Moq;

namespace Backend.Tests.Tests
{
    [TestFixture]
    public class FfmpegAudioConversionServiceTests
    {
        private Mock<IProcessRunner> _runner;
        private FfmpegAudioConversionService _service;

        [SetUp]
        public void Setup()
        {
            _runner = new Mock<IProcessRunner>();
            _service = new FfmpegAudioConversionService(_runner.Object, "m4a");
        }

        [Test]
        public async Task Should_ReturnSamePath_WhenAlreadyM4A()
        {
            var input = "song.m4a";

            var result = await _service.ConvertAsync(input, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.That(result.Value, Is.EqualTo(input));

            _runner.Verify(x => x.RunAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task Should_Convert_WhenNotM4A()
        {
            _runner.Setup(x => x.RunAsync(
                "ffmpeg",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var input = "song.mp3";

            var result = await _service.ConvertAsync(input, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.That(result.Value, Does.EndWith(".m4a"));

            _runner.Verify(x => x.RunAsync(
                "ffmpeg",
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Should_Fail_WhenRunnerReturnsError()
        {
            _runner.Setup(x => x.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.ConvertAsync("song.mp3", CancellationToken.None);

            Assert.IsFalse(result.IsSuccess);
        }
    }
}