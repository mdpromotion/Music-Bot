using Backend.Domain.Enums;
using Backend.Domain.ValueObjects;

namespace Backend.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
        public required TaskMetadata Metadata { get; set; }
    }
}
