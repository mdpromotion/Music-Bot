using Telegram.DTO;

namespace Telegram.Strategies
{
    public class StartStrategy : IUpdateStrategy
    {
        public UpdateType Type => UpdateType.Command;
        public string? Command => "/start";

        public Task HandleAsync(TelegramUpdateData update)
        {
            return Task.CompletedTask;
        }
    }
}
