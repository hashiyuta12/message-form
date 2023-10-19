using MagicOnion.Server.Hubs;
using RealtimeServer.Shared;

namespace RealtimeServer.Tools
{
    public class RealtimeHub : StreamingHubBase<IRealtimeHub, IRealtimeHubReceiver>, IRealtimeHub
    {
        #region Member
        private IGroup room;
        private Client client;
        private IInMemoryStorage<Client> storage;
        #endregion

        #region Method
        // MagicOnion Server => Client
        // Join Procedure
        public async Task<Client[]> JoinAsync(string roomName, string Address)
        {
            client = new Client() { Address = Address };
            (room, storage) = await Group.AddAsync(roomName, client);

            Console.WriteLine($"Info: {DateTime.Now} - Join new client.");
            return storage.AllValues.ToArray();
        }

        // Leave Procedure
        public async Task LeaveAsync()
        {
            Health health = new Health() { Address = client.Address, isActive = false };
            await room.RemoveAsync(Context);
            Broadcast(room).OnLeave(client);
            Broadcast(room).OnHeartbeat(health);

            Console.WriteLine($"Info: {DateTime.Now} - Leave the client.");
        }

        // Receving New Message Procedure
        public async Task ReceiveAsync(byte[] data)
        {
            Broadcast(room).OnReceive(data);
            Console.WriteLine($"Info: {DateTime.Now} - Receive new message.");
        }

        // Heartbeat Procedure
        public async Task HeartbeatAsync(string roomName, string Address)
        {
            Health health = new Health() { Address = Address, isActive = true};
            Broadcast(room).OnHeartbeat(health);

            Console.WriteLine($"Info: {DateTime.Now} - {Address} is active.");
        }
        #endregion
    }
}
