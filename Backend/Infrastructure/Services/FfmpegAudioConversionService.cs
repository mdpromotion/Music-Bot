using Backend.Application.Common;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Interfaces;

namespace Backend.Infrastructure.Services
{
    public class FfmpegAudioConversionService : IAudioConversionService
    {
        private readonly IProcessRunner _processRunner;
        private readonly string _targetFormat;

        public FfmpegAudioConversionService(
            IProcessRunner processRunner,
            string targetFormat = "m4a")
        {
            _processRunner = processRunner;
            _targetFormat = targetFormat;
        }

        private bool IsTargetFormat(string path)
        {
            return Path.GetExtension(path)
                .TrimStart('.')
                .Equals(_targetFormat, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<Result<string>> ConvertAsync(string inputPath, CancellationToken ct)
        {
            try
            {
                if (IsTargetFormat(inputPath))
                    return Result<string>.Success(inputPath);

                var outputPath = Path.ChangeExtension(inputPath, _targetFormat);

                var args = $"-i \"{inputPath}\" -c:a aac -b:a 192k \"{outputPath}\" -y";

                var exitCode = await _processRunner.RunAsync("ffmpeg", args, ct);

                if (exitCode != 0)
                    return Result<string>.Failure("FFmpeg conversion failed");

                return Result<string>.Success(outputPath);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Audio conversion failed: {ex.Message}");
            }
        }
    }
}