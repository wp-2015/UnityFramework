using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class Proxy
    {
        
        protected void AddServerListener(MSGTYPE type, Action<IMessage> handle)
        {
            ServerListener.AddHandle(type, handle);
        }

        public void SendSocket(Packet packet)
        {
            NetworkManager.SendMessage(packet);
        }

        public PacketEntity<T> CreatePacket<T>(MSGTYPE msgType) where T : IMessage, new()
        {
            var packet = ObjectPool<PacketEntity<T>>.Instance.Get();
            packet.ID = (short)msgType;
            packet.message = Activator.CreateInstance<T>();
            return packet;
        }
}
}
