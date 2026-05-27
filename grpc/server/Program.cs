using System.Net;
using HelloWorld;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Parse("127.0.0.1"), 5000);
    serverOptions.Listen(IPAddress.Parse("127.0.0.1"), 5001, listenOptions =>
    {
        // Use RSA algorithm without password.
        //
        // The algorithm identified by '1.3.101.112' is unknown,
        // not valid for the requested usage, or was not handled.
        var _ = listenOptions.UseHttps("cert.pfx");
    });
});

var app = builder.Build();
app.MapGrpcService<ApiService>();

app.Run();
