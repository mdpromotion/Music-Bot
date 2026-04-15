using Backend.Application.Interfaces;

namespace Backend.Application.UseCases
{
    public class ValidateFileUseCase
    {
        private readonly IFileValidationService _fileValidationService;

        public ValidateFileUseCase(IFileValidationService fileValidationService)
        {
            _fileValidationService = fileValidationService;
        }

        public bool Validate(string path) 
        {
            return _fileValidationService.EnsureFileExists(path);
        }

    }
}
