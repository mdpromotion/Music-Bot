using Backend.Domain.ValueObjects;

namespace Backend.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public TaskStatus Status { get; set; }
        public List<TaskMetadata> Metadata { get; set; } = [];
    }
}
