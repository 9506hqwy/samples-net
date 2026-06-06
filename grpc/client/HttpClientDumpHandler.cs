internal class HttpClientDumpHandler : HttpClientHandler
{
    internal HttpClientDumpHandler()
        : base()
    {
        this.AllowAutoRedirect = false;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var res = base.SendAsync(request, cancellationToken);
        return res.ContinueWith(
            this.Dumper,
            cancellationToken,
            TaskContinuationOptions.NotOnFaulted,
            TaskScheduler.Default);
    }

#pragma warning disable CA1303
    private HttpResponseMessage Dumper(Task<HttpResponseMessage> task)
    {
        var message = task.Result;
        Console.WriteLine("------------------------------");
        Console.WriteLine($"StatusCode = {message.StatusCode}");
        foreach (var header in message.Headers)
        {
            Console.WriteLine($"{header.Key} = {string.Join(",", header.Value)}");
        }

        Console.WriteLine("------------------------------");
        return message;
    }
}
#pragma warning restore CA1303
