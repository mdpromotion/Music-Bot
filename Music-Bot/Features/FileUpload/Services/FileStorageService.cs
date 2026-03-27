namespace Backend.Features.FileUpload.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public FileStorageService()
        {
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var fileId = Guid.NewGuid().ToString();
            var filePath = Path.Combine(_storagePath, fileId);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileId;
        }
        
        public async Task<StoredFile?> GetFileAsync(string fileId)
        {
            var filePath = Path.Combine(_storagePath, fileId);
            if (!File.Exists(filePath))
                return null;

            var content = await File.ReadAllBytesAsync(filePath);
            return new StoredFile
            {
                FileName = fileId,
                Content = content,
                ContentType = "application/octet-stream"
            };
        }
    }
    public class StoredFile
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = [];
        public string ContentType { get; set; } = string.Empty;
    }
}
