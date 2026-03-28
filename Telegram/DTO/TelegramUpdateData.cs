namespace Telegram.DTO
{
    public record TelegramUpdateData(
        long UserId,
        string? TextCommand = null,
        IFormFile? File = null,
        string? FileType = null
    );
}
