using Telegram.DTO;

namespace Telegram.Strategies
{
    public interface IUpdateStrategy
    {
        UpdateType Type { get; }
        string? Command { get; }
        Task HandleAsync(TelegramUpdateData update);
    }
}