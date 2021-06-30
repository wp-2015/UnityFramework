using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class NetworkManager
    {
        private static NetwrokChannel channel;
        public void CreateNetworkChannel()
        {
            channel = new NetwrokChannel();
        }

        public static void SendMessage(Packet packet)
        {
            channel.Send(packet);
        }
    }
}
