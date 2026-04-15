using Backend.Application.Common;
using Backend.Application.Interfaces;

namespace Backend.Application.UseCases
{
    public class ConvertAudioUseCase
    {
        private readonly IAudioConversionService _conversionService;

        public ConvertAudioUseCase(IAudioConversionService conversionService)
        {
            _conversionService = conversionService;
        }

        public Task<Result<string>> Execute(string path, CancellationToken ct)
        {
            return _conversionService.ConvertAsync(path, ct);
        }
    }
}
