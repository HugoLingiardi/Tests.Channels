using System.Threading.Channels;
using FastEndpoints;
using FastEndpoints.Swagger;
using Tests.Channels;
using Tests.Channels.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ => Channel.CreateBounded<Message>(new BoundedChannelOptions(5)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleReader = false,
    SingleWriter = true,
    AllowSynchronousContinuations = true
}));
builder.Services.AddSingleton(x => x.GetRequiredService<Channel<Message>>().Reader);
builder.Services.AddSingleton(x => x.GetRequiredService<Channel<Message>>().Writer);

builder.Services.AddHostedService<WorkerChannelConsumer>();

builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();

var app = builder.Build();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();