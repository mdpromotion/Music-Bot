using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("tune/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskQueueService _taskQueueService;

        public TasksController(ITaskQueueService taskQueueService)
        {
            _taskQueueService = taskQueueService;
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] CreateTaskRequest request)
        {
            var metadata = new TaskMetadata
            {
                SoundPath = request.SoundPath,
                Author = request.Author,
                Title = request.Title
            };

            var task = _taskQueueService.CreateTask(metadata);

            if (task.IsFailure)
                return BadRequest(task.Error);

            return Ok(task.Value);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            var success = _taskQueueService.DeleteTask(id);
            if (!success)
                return NotFound("Task not found");

            return NoContent();
        }
    }
}
