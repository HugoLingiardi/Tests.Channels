using System.Threading.Channels;
using FastEndpoints;
using Tests.Channels.Models;

namespace Tests.Channels.Endpoints.Producer;

public class ProducerEndpoint : Endpoint<ProducerData>
{
    private readonly ILogger<ProducerEndpoint> _logger;
    private readonly ChannelWriter<Message> _channelWriter;

    public ProducerEndpoint(
        ILogger<ProducerEndpoint> logger,
        ChannelWriter<Message> channelWriter)
    {
        _logger = logger;
        _channelWriter = channelWriter;
    }

    public override void Configure()
    {
        Post("/producer");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ProducerData req, CancellationToken ct)
    {
        var message = new Message(Guid.NewGuid(), DateTime.Now, req.Data);
        
        _logger.LogInformation("Producing message {@Message}", message);

        await _channelWriter.WriteAsync(message, ct);
        await SendOkAsync(ct);
    }
}