using System.Threading.Channels;
using Tests.Channels.Models;

namespace Tests.Channels;

public class WorkerChannelConsumer : BackgroundService
{
    private readonly ILogger<WorkerChannelConsumer> _logger;
    private readonly ChannelReader<Message> _channelReader;

    private object _lock;
    private int _total;

    public WorkerChannelConsumer(
        ILogger<WorkerChannelConsumer> logger,
        ChannelReader<Message> channelReader)
    {
        _logger = logger;
        _channelReader = channelReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting reader...");

        var tasks = Enumerable.Range(0, 10).Select(i => Task.Factory.StartNew(async (data) =>
        {
            var (ct, id) = (ValueTuple<CancellationToken, int>)data! ;
            
            while (!ct.IsCancellationRequested)
            {
                if (await _channelReader.WaitToReadAsync(stoppingToken) &&
                    await _channelReader.ReadAsync(stoppingToken) is var message)
                {
                    var total = Interlocked.Increment(ref _total);

                    _logger.LogInformation("Message read {Id} {Total} {@Message} ", id.ToString(), total.ToString(), message);
                }
            }
        }, (stoppingToken, i), stoppingToken));
        
        await Task.WhenAll(tasks);
    }
}