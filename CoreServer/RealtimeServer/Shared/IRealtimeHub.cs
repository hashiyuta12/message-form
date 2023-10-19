using MagicOnion;
using MessagePack;

namespace RealtimeServer.Shared
{
    // Callback to server
    public interface IRealtimeHub : IStreamingHub<IRealtimeHub, IRealtimeHubReceiver>
    {
        Task<Client[]> JoinAsync(string roomName, string Address);
        Task LeaveAsync();
        Task ReceiveAsync(byte[] data);
        Task HeartbeatAsync(string roomName, string Address);
    }

    // Callbacks from the server.
    public interface IRealtimeHubReceiver
    {
        void OnJoin(Client client);
        void OnLeave(Client client);
        void OnReceive(byte[] data);
        void OnHeartbeat(Health health);
    }

    // For participating clients.
    [MessagePackObject]
    public class Client
    {
        [Key(0)]
        public string Address { get; set; }
    }

    // For the client's heartbeat.
    [MessagePackObject]
    public class Health
    {
        [Key(0)]
        public string Address { get; set; }
        [Key(1)]
        public bool isActive { get; set; }
    }
}
