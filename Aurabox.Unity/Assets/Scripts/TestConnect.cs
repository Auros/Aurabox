using LiteNetLib;
using UnityEngine;

namespace Aurabox
{
    public class TestConnect : MonoBehaviour
    {
        private NetManager _server;
        private EventBasedNetListener _listener;
        
        private void Awake()
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
            _server.Start();
            _server.Connect("localhost", 10300, "Hello!");
        }

        private void Update()
        {
            _server.PollEvents();
        }

        private void OnEnable()
        {
            _listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
        }

        private static void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
        {
            print($"We got: {reader.GetString(100)}");
        }

        private void OnDisable()
        {
            _listener.NetworkReceiveEvent -= ListenerOnNetworkReceiveEvent;
        }
    }
}