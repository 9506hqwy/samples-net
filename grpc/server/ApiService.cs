using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

#pragma warning disable IDE0130
namespace HelloWorld;
#pragma warning disable IDE0130

#pragma warning disable CA1812
internal sealed class ApiService : Api.ApiBase
#pragma warning disable CA1812
{
    public override Task<ScalarValueTypes> Call(ScalarValueTypes request, ServerCallContext context)
    {
        var res = request.Clone();
        return Task.FromResult(res);
    }

    public override async Task Download(Empty request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        try
        {
            var count = 0;
            while (true)
            {
                await Task.Delay(1 * 1000).ConfigureAwait(false);

                await responseStream.WriteAsync(new Message { Message_ = $"count {count}" }).ConfigureAwait(false);

                count += 1;
            }
        }
#pragma warning disable CA1031
        catch
        {
        }
#pragma warning disable CA1031
    }

    public override async Task<Empty> Upload(IAsyncStreamReader<Message> requestStream, ServerCallContext context)
    {
        await foreach (var msg in requestStream.ReadAllAsync().ConfigureAwait(false))
        {
            await Console.Out.WriteLineAsync($"Upload: {msg.Message_}").ConfigureAwait(false);
        }

        return new Empty();
    }

    public override async Task Async(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        await foreach (var msg in requestStream.ReadAllAsync().ConfigureAwait(false))
        {
            await responseStream.WriteAsync(msg).ConfigureAwait(false);
        }
    }
}
