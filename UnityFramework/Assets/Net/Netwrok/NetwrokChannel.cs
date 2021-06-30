using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class NetwrokChannel
    {
        private Queue<Packet> sendPacketPool = new Queue<Packet>();

        public void Send(Packet packet)
        {
            sendPacketPool.Enqueue(packet);
        }

        public void Update()
        {
            while(sendPacketPool.Count > 0)
            {
                lock (sendPacketPool)
                {
                    var packet = sendPacketPool.Dequeue();
                }
            }
        }
    }
}
