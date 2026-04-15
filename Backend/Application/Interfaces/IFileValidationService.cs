namespace Backend.Application.Interfaces
{
    public interface IFileValidationService
    {
        bool EnsureFileExists(string path);
    }
}
