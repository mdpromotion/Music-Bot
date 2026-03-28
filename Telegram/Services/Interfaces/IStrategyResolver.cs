using Telegram.DTO;
using Telegram.Strategies;

namespace Telegram.Services.Interfaces
{
    public interface IStrategyResolver
    {
        IUpdateStrategy? Resolve(TelegramUpdateData updateData);
    }
}