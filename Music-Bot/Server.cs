using Backend.Features.FileUpload.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IFileStorageService, FileStorageService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
