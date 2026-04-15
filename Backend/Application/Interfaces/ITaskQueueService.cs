using Backend.Application.Common;
using Backend.Domain.Entities;
using Backend.Domain.ValueObjects;

namespace Backend.Application.Interfaces
{
    public interface ITaskQueueService
    {
        Result<TaskItem> CreateTask(TaskMetadata metadata);
        bool DeleteTask(Guid id);
        Result<TaskItem> GetTask(Guid id);
    }
}
