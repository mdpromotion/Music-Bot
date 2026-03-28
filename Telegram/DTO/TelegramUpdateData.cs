namespace Telegram.DTO
{
    public record TelegramUpdateData(
        long UserId,
        UpdateType Type,
        string? TextCommand = null,
        IFormFile? File = null
    );
}
