using Backend.Application.Common;
using Backend.Domain.ValueObjects;

namespace Backend.Infrastructure.Interfaces
{
    public interface IAudioOrchestrator
    {
        Task<Result> ProcessAsync(TaskMetadata input, CancellationToken ct);
    }
}
