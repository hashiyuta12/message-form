using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sender.Tools
{
    public class udpControl
    {
        #region Member
        private IPEndPoint localEP;
        private UdpClient client;
        #endregion

        #region Constructor
        public udpControl(IPEndPoint ep)
        {
            this.localEP = ep;
            this.client = new UdpClient(localEP);
        }
        #endregion

        #region Method
        public void SendAsync(IPEndPoint remoteEP, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.SendAsync(data, data.Length, remoteEP);
        }

        public void Close()
        {
            client.Close();
        }
        #endregion
    }
}
