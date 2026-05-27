using Grpc.Core;
using Grpc.Net.Client;
using HelloWorld;
using WellKnownTypes = Google.Protobuf.WellKnownTypes;

using var httpHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
};
#pragma warning disable CA5400
using var httpClient = new HttpClient(httpHandler);
#pragma warning restore CA5400

using var channel = GrpcChannel.ForAddress(
    "https://127.0.0.1:5001",
    new GrpcChannelOptions { HttpClient = httpClient });
var client = new Api.ApiClient(channel);

// Call
var res = await client.CallAsync(new ScalarValueTypes());
Console.WriteLine($"{res}");

// Download
var downloadSteam = client.Download(new WellKnownTypes.Empty());
var count = 0;
await foreach (var msg in downloadSteam.ResponseStream.ReadAllAsync().ConfigureAwait(false))
{
    Console.WriteLine($"Message: {msg.Message_}");
    count += 1;

    if (count > 3)
    {
        break;
    }
}

// Upload
var uploadSteam = client.Upload();
await uploadSteam.RequestStream.WriteAsync(new Message { Message_ = $"count 1" }).ConfigureAwait(false);
await uploadSteam.RequestStream.WriteAsync(new Message { Message_ = $"count 2" }).ConfigureAwait(false);
await uploadSteam.RequestStream.WriteAsync(new Message { Message_ = $"count 3" }).ConfigureAwait(false);
await uploadSteam.RequestStream.CompleteAsync().ConfigureAwait(false);
var uploadRes = await uploadSteam.ResponseAsync.ConfigureAwait(false);
Console.WriteLine($"{uploadRes}");

// Async
var biStream = client.Async();
await biStream.RequestStream.WriteAsync(new Message { Message_ = $"count 1" }).ConfigureAwait(false);
await biStream.RequestStream.WriteAsync(new Message { Message_ = $"count 2" }).ConfigureAwait(false);
await biStream.RequestStream.WriteAsync(new Message { Message_ = $"count 3" }).ConfigureAwait(false);
await biStream.RequestStream.CompleteAsync().ConfigureAwait(false);
await foreach (var msg in biStream.ResponseStream.ReadAllAsync().ConfigureAwait(false))
{
    Console.WriteLine($"Message: {msg.Message_}");
}
