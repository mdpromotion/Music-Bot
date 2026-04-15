using Backend.Application.Common;
using Backend.Domain.ValueObjects;

namespace Backend.Application.Interfaces
{
    public interface IMetadataService
    {
        Task<Result> UpdateMetadataAsync(string filePath, SoundMetadata metadata, CancellationToken ct);
    }
}
