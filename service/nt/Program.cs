var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
