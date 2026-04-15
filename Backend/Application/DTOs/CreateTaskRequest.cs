namespace Backend.Application.DTOs
{
    public class CreateTaskRequest
    {
        public required string SoundPath { get; set; }
        public required string Author { get; set; }
        public required string Title { get; set; }
    }
}
