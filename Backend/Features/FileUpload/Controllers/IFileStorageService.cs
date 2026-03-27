namespace Backend.Features.FileUpload.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<StoredFile?> GetFileAsync(string fileId);
    }
}
