using Backend.Application.Common;
using Backend.Application.Interfaces;
using Backend.Domain.ValueObjects;

namespace Backend.Application.UseCases
{
    public class UpdateMetadataUseCase
    {
        private readonly IMetadataService _metadataService;

        public UpdateMetadataUseCase(IMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        public Task<Result> Execute(string path, SoundMetadata metadata, CancellationToken ct)
        {
            return _metadataService.UpdateMetadataAsync(path, metadata, ct);
        }
    }
}
