using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Services
{
    public class FileValidationService : IFileValidationService
    {
        public bool EnsureFileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
