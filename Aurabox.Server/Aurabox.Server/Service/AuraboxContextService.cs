using LiteNetLib;

namespace Aurabox.Server.Service;

public class AuraboxContextService
{
    public NetManager Server { get; }
    public EventBasedNetListener Listener { get; } = new();

    public AuraboxContextService()
    {
        Server = new NetManager(Listener);
    }
}