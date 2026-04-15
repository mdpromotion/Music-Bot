using Backend.Application.Interfaces;
using Backend.Application.Orchestrator;
using Backend.Application.Services;
using Backend.Application.Workers;
using Backend.Domain.ValueObjects;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProcessRunner, ProcessRunner>();
builder.Services.AddSingleton<IFileValidationService, FileValidationService>();
builder.Services.AddSingleton<IAudioConversionService, FfmpegAudioConversionService>();
builder.Services.AddSingleton<IMetadataService, FfmpegMetadataService>();


builder.Services.AddSingleton<ITaskQueueService, TaskQueueService>();
builder.Services.AddScoped<IAudioOrchestrator, AudioProcessingOrchestrator>();

builder.Services.AddHostedService<TaskWorker>();

var app = builder.Build();

app.MapGet("/", () => "Backend is running");

app.MapPost("/tune/tasks", (TaskMetadata metadata, ITaskQueueService queue) =>
{
    var result = queue.CreateTask(metadata);

    if (!result.IsSuccess)
        return Results.BadRequest(result.Error);

    return Results.Ok(result.Value);
});

app.MapGet("/tune/tasks/{id:guid}", (Guid id, ITaskQueueService queue) =>
{
    var result = queue.GetTask(id);

    if (!result.IsSuccess)
        return Results.NotFound(result.Error);

    return Results.Ok(result.Value);
});

app.Run();