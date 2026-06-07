var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSystemd();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
