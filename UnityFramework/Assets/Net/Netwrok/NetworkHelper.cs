using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class NetworkHelper
    {
        public void SendPacket(Packet packet)
        {
            //获得ID字节数组
            byte[] idBytes = BitConverter.GetBytes(packet.ID);
            //
        }
    }
}
