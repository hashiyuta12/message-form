using System.Diagnostics;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

namespace CoreServer.Tools
{
    public class magiconionControl
    {
        #region Member
        private GrpcChannel channel;
        private RealtimeHub realtimeHub = new RealtimeHub();
        private string url;
        private string roomName;
        private string Address;
        #endregion

        #region Constructor
        public magiconionControl(string url, string roomName, string Address)
        {
            this.url = url;
            this.roomName = roomName;
            this.Address = Address;
        }
        #endregion

        #region Method
        // Create gRPC channel
        public async Task<bool> createChannel(string url)
        {
            var defaultMethodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(10),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                }
            };
            channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
            });
            return await realtimeHub.ConnectAsync(channel);
        }

        // Join MagicOnion server
        public async void Connect()
        {
            bool status = false;
            while(!status)
            {
                Console.WriteLine($"Info: {DateTime.Now} - Connect to the MagicOnion server.");
                status = await createChannel(url);
            }
            Console.WriteLine($"Info: {DateTime.Now} - OK to connect to the MagicOnion server.");
            await realtimeHub.JoinAsync(roomName, Address);
        }

        // Leave MagicOnion server
        public async void Close()
        {
            if (realtimeHub == null) return;
            await realtimeHub.LeaveAsync();
        }

        // Output gRPC channel status
        public ConnectivityState channelStatus()
        {
            return channel.State;
        }

        // Execute heatbeat sequence
        public async void Heartbeat()
        {
            await realtimeHub.HeartbeatAsync(roomName, Address);
        }

        // Execute sending message sequence
        public async void Post(byte[] data)
        {
            await realtimeHub.PostAsync(data);
        }
        #endregion
    }
}
