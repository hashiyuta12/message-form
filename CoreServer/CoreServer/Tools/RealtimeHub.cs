using Grpc.Core;
using MagicOnion.Client;
using RealtimeServer.Shared;

namespace CoreServer.Tools
{
    public class RealtimeHub : IRealtimeHubReceiver
    {
        #region Member
        private IRealtimeHub realtimeServer;
        private Dictionary<string, Client> users = new Dictionary<string, Client>();
        #endregion

        #region Method
        public async Task<bool> ConnectAsync(ChannelBase grpcChannel)
        {
            try
            {
                realtimeServer = await StreamingHubClient.ConnectAsync<IRealtimeHub, IRealtimeHubReceiver>(grpcChannel, this);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<Client> JoinAsync(string roomName, string Address)
        {
            var roomUsers = await realtimeServer.JoinAsync(roomName, Address);
            foreach(var user in roomUsers)
            {
                (this as IRealtimeHubReceiver).OnJoin(user);
            }
            Console.WriteLine($"Info: {DateTime.Now} - Join the MagicOnion Server.");
            return users[Address];
        }

        public async Task PostAsync(byte[] data)
        {
            try
            {
                await realtimeServer.ReceiveAsync(data);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task HeartbeatAsync(string roomName, string Address)
        {
            if (realtimeServer == null) return;
            try
            {
                await realtimeServer.HeartbeatAsync(roomName, Address);
            }
            catch(Exception ex)
            {

            }
        }

        public Task LeaveAsync()
        {
            if (realtimeServer != null)
            {
                realtimeServer.LeaveAsync();
                realtimeServer.DisposeAsync();
            }
            Console.WriteLine($"Info: {DateTime.Now} - Leave the MagicOnion Server.");
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return realtimeServer.DisposeAsync();
        }

        public Task WaitForDisconnect()
        {
            return realtimeServer.WaitForDisconnect();
        }

        void IRealtimeHubReceiver.OnJoin(Client client)
        {
            users[client.Address] = client;
        }

        void IRealtimeHubReceiver.OnLeave(Client client)
        {
            if(users.TryGetValue(client.Address, out var value))
            {
                users.Remove(client.Address);
            }
        }

        void IRealtimeHubReceiver.OnReceive(byte[] data)
        {

        }

        void IRealtimeHubReceiver.OnHeartbeat(Health health)
        {

        }
        #endregion
    }
}
