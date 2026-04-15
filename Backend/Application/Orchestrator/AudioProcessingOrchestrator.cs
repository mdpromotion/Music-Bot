using Backend.Application.Common;
using Backend.Application.UseCases;
using Backend.Domain.ValueObjects;
using Backend.Infrastructure.Interfaces;

namespace Backend.Application.Orchestrator
{
    public class AudioProcessingOrchestrator : IAudioOrchestrator
    {
        private readonly ValidateFileUseCase _validateFile;
        private readonly ConvertAudioUseCase _convertAudio;
        private readonly UpdateMetadataUseCase _updateMetadata;

        public AudioProcessingOrchestrator(
            ValidateFileUseCase validateFile,
            ConvertAudioUseCase convertAudio,
            UpdateMetadataUseCase updateMetadata)
        {
            _validateFile = validateFile;
            _convertAudio = convertAudio;
            _updateMetadata = updateMetadata;
        }

        public async Task<Result> ProcessAsync(TaskMetadata input, CancellationToken ct)
        {
            var fileExists = _validateFile.Validate(input.SoundPath);

            if (!fileExists)
                return Result.Failure("File does not exist.");

            var convertResult = await _convertAudio.Execute(input.SoundPath, ct);

            if (!convertResult.IsSuccess || convertResult.Value == null)
                return Result.Failure(convertResult.Error);

            var metadataResult = await _updateMetadata.Execute(
                convertResult.Value,
                new SoundMetadata
                {
                    Author = input.Author,
                    Title = input.Title
                },
                ct);

            if (!metadataResult.IsSuccess)
                return Result.Failure(metadataResult.Error);

            return Result.Success();
        }
    }
}
