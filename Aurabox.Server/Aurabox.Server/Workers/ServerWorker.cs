using Aurabox.Server.Service;

namespace Aurabox.Server.Workers;

public class ServerWorker : BackgroundService
{
    private readonly AuraboxContextService _auraboxContextService;
    
    public ServerWorker(AuraboxContextService auraboxContextService)
    {
        _auraboxContextService = auraboxContextService;
    }
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _auraboxContextService.Server.Start(10300);
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