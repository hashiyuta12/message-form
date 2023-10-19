using System.Net;
using System.Net.Sockets;

namespace CoreServer.Tools
{
    public delegate void ReceiveEventHandler(byte[] data);
    public class communicationControl
    {
        #region Member
        private IPEndPoint localEP;
        private UdpClient udpClient;
        public event ReceiveEventHandler propagation = v => { };
        #endregion

        #region Constructor
        public communicationControl(IPEndPoint ep)
        {
            this.localEP = ep;
        }
        #endregion

        #region Method
        public void Open()
        {
            udpClient = new UdpClient(localEP);
            beginReceive();
        }

        public void Close()
        {
            if (udpClient != null) udpClient.Client.DisconnectAsync(new SocketAsyncEventArgs());
        }

        public void Dispose()
        {
            if(udpClient != null) udpClient.Close();
            GC.SuppressFinalize(this);
        }

        private void beginReceive()
        {
            udpClient.BeginReceive(onReceived, null);
        }

        private void onReceived(IAsyncResult ar)
        {
            byte[] data = null;
            IPEndPoint remoteEP = null;
            try
            {
                data = udpClient.EndReceive(ar, ref remoteEP);
                if (data.Length == 0) return;
                beginReceive();
            }
            catch (Exception ex)
            {
                Close();
                Thread.Sleep(300);
                Open();
                beginReceive();
            }
            propagation(data);
        }

        public static bool isServer(IPAddress targetIP)
        {
            bool ret = false;
            IPAddress[] device = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in device)
            {
                if(address.Equals(targetIP))
                {
                    ret = true;
                }
            }
            return ret;
        }
        #endregion
    }
}
