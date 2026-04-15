using Backend.Application.Common;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace Backend.Application.Services
{
    public class TaskQueueService : ITaskQueueService
    {
        private readonly ConcurrentDictionary<Guid, TaskItem> _tasks = new();
        private readonly object _lock = new();

        public Result<TaskItem> CreateTask(TaskMetadata metadata)
        {
            if (metadata == null)
                return Result<TaskItem>.Failure("Metadata is required");

            if (string.IsNullOrWhiteSpace(metadata.SoundPath) ||
                string.IsNullOrWhiteSpace(metadata.Author) ||
                string.IsNullOrWhiteSpace(metadata.Title))
            {
                return Result<TaskItem>.Failure("All fields are required");
            }

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Status = Status.Pending,
                Metadata = metadata
            };

            _tasks[task.Id] = task;

            return Result<TaskItem>.Success(task);
        }

        public bool DeleteTask(Guid id)
        {
            return _tasks.TryRemove(id, out _);
        }

        public Result<TaskItem> GetTask(Guid id)
        {
            if (!_tasks.TryGetValue(id, out var task))
                return Result<TaskItem>.Failure("Task not found");

            return Result<TaskItem>.Success(task);
        }

        public bool TryGetAndLockPendingTask(out TaskItem? task)
        {
            lock (_lock)
            {
                var kvp = _tasks.FirstOrDefault(x => x.Value.Status == Status.Pending);

                if (kvp.Value == null)
                {
                    task = null;
                    return false;
                }

                _tasks.TryRemove(kvp.Key, out _);

                task = new TaskItem
                {
                    Id = kvp.Value.Id,
                    Metadata = kvp.Value.Metadata,
                    Status = Status.Processing
                };

                _tasks[task.Id] = task;
                return true;
            }
        }

        public void Update(TaskItem task)
        {
            _tasks[task.Id] = task;
        }
    }
}