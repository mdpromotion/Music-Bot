namespace Backend.Domain.ValueObjects
{
    public class TaskMetadata
    {
        public required string SoundPath { get; set; }
        public required string Author { get; set; }
        public required string Title { get; set; }
    }
}
