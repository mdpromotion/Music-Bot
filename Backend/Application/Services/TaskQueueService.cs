using Backend.Application.Common;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.ValueObjects;

namespace Backend.Application.Services
{
    public class TaskQueueService : ITaskQueueService
    {
        private readonly Dictionary<Guid, TaskItem> _tasks = new();

        public Result<TaskItem> CreateTask(TaskMetadata metadata)
        {
            if (metadata == null)
                return Result<TaskItem>.Failure("Metadata is required");

            if (string.IsNullOrEmpty(metadata.SoundPath) ||
                string.IsNullOrEmpty(metadata.Author) ||
                string.IsNullOrEmpty(metadata.Title))
            {
                return Result<TaskItem>.Failure("All metadata fields are required");
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
            return _tasks.Remove(id);
        }

        public Result<TaskItem> GetTask(Guid id)
        {
            if (!_tasks.TryGetValue(id, out var task))
                return Result<TaskItem>.Failure("Task not found");

            return Result<TaskItem>.Success(task);
        }
    }
}
