using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class Proxy
    {
        public void SendSocket(Packet packet)
        {
            NetworkManager.SendMessage(packet);
        }
    }
}
