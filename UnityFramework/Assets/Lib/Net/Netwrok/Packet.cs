using Google.Protobuf;
using System;

namespace Netwrok
{
    public class Packet
    {
        public short ID;
        public IMessage message;
    }

    public class PacketEntity<T> : Packet where T : IMessage
    {
        public T msg
        {
            get { return (T)message; }
        }
    }
}
