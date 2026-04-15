using Backend.Application.Common;

namespace Backend.Application.Interfaces
{
    public interface IAudioConversionService
    {
        Task<Result<string>> ConvertAsync(string inputPath, CancellationToken ct);
    }
}
