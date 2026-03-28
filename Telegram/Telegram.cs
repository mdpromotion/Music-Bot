var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

app.MapGet("/", () => "Hello World!");

app.Run();
