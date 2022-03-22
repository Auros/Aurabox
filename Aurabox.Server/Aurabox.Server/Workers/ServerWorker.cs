using Aurabox.Server.Service;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Aurabox.Server.Workers;

public class ServerWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly AuraboxContextService _auraboxContextService;
    
    public ServerWorker(ILogger<ServerWorker> logger, AuraboxContextService auraboxContextService)
    {
        _logger = logger;
        _auraboxContextService = auraboxContextService;
    }
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _auraboxContextService.Server.Start(10300);
        
        _auraboxContextService.Listener.ConnectionRequestEvent += request =>
        {
            if(_auraboxContextService.Server.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey("Hello!");
            else
                request.Reject();
        };

        _auraboxContextService.Listener.PeerConnectedEvent += peer =>
        {
            _logger.LogInformation("We got connection: {EndPoint}", peer.EndPoint); // Show peer ip
            NetDataWriter writer = new();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
        };
        
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _auraboxContextService.Server.PollEvents();
            await Task.Delay(15, default);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _auraboxContextService.Server.Stop();
        return base.StopAsync(cancellationToken);
    }
}