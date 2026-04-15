using Backend.Application.Interfaces;
using Backend.Domain.Enums;
using Backend.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Workers
{
    public class TaskWorker : BackgroundService
    {
        private readonly ITaskQueueService _queue;
        private readonly IAudioOrchestrator _orchestrator;
        private readonly ILogger<TaskWorker> _logger;

        public TaskWorker(
            ITaskQueueService queue,
            IAudioOrchestrator orchestrator,
            ILogger<TaskWorker> logger)
        {
            _queue = queue;
            _orchestrator = orchestrator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryGetAndLockPendingTask(out var task))
                    {
                        _logger.LogInformation($"Picked task {task!.Id}");

                        try
                        {
                            await _orchestrator.ProcessAsync(task.Metadata, stoppingToken);

                            task.Status = Status.Completed;
                            _queue.Update(task);

                            _queue.DeleteTask(task.Id);

                            _logger.LogInformation($"Task {task.Id} completed");
                        }
                        catch (Exception ex)
                        {
                            task.Status = Status.Failed;
                            _queue.Update(task);

                            _logger.LogError(ex, $"Task {task.Id} failed");
                        }
                    }
                    else
                    {
                        await Task.Delay(500, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker loop error");
                }
            }
        }
    }
}