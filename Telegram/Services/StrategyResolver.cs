using Telegram.DTO;
using Telegram.Services.Interfaces;
using Telegram.Strategies;

namespace Telegram.Services
{
    public class StrategyResolver : IStrategyResolver
    {
        private readonly Dictionary<string, IUpdateStrategy> _commandStrategies;
        private readonly Dictionary<UpdateType, IUpdateStrategy> _typeStrategies;

        public StrategyResolver(IEnumerable<IUpdateStrategy> strategies)
        {
            _commandStrategies = strategies
                .Where(s => s.Type == UpdateType.Command && s.Command != null)
                .ToDictionary(s => s.Command!);

            _typeStrategies = strategies
                .Where(s => s.Type != UpdateType.Command)
                .ToDictionary(s => s.Type);
        }

        public IUpdateStrategy? Resolve(TelegramUpdateData updateData)
        {
            if (updateData.Type == UpdateType.Command && updateData.TextCommand != null)
            {
                _commandStrategies.TryGetValue(updateData.TextCommand, out var commandStrategy);
                return commandStrategy;
            }

            _typeStrategies.TryGetValue(updateData.Type, out var typeStrategy);
            return typeStrategy;
        }

    }
}
