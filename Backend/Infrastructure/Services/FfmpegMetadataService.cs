using Backend.Application.Common;
using Backend.Application.Interfaces;
using Backend.Domain.ValueObjects;
using Backend.Infrastructure.Interfaces;

namespace Backend.Infrastructure.Services
{
    public class FfmpegMetadataService : IMetadataService
    {
        private readonly IProcessRunner _processRunner;

        public FfmpegMetadataService(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public async Task<Result> UpdateMetadataAsync(string filePath, SoundMetadata metadata, CancellationToken ct)
        {
            try
            {
                var tempFile = Path.Combine(
                    Path.GetDirectoryName(filePath)!,
                    "temp_" + Path.GetFileName(filePath));

                var args =
                    $"-i \"{filePath}\" " +
                    $"-metadata title=\"{metadata.Title}\" " +
                    $"-metadata artist=\"{metadata.Author}\" " +
                    $"\"{tempFile}\"";

                var exitCode = await _processRunner.RunAsync("ffmpeg", args, ct);

                if (exitCode != 0)
                    return Result.Failure("Metadata update failed");

                File.Replace(tempFile, filePath, null);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}