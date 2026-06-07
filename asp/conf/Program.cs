var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("server.json", false, true);
builder.WebHost.UseKestrel(options =>
{
    var _ = options.Configure(builder.Configuration.GetSection("Kestrel"));
});

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
