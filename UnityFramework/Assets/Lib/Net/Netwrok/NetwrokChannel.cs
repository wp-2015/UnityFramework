using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class NetwrokChannel
    {
        //Packet缓存池
        private Queue<Packet> sendPacketPool = new Queue<Packet>();
        //将要发送的数据放入流
        private const int DefaultBufferLength = 1024 * 64;
        private MemoryStream Stream = new MemoryStream(DefaultBufferLength);
        //Socket
        protected Socket Socket;

        public NetwrokChannel()
        {
            Stream.Position = 0;
        }

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
                    NetworkHelper.SendPacket(sendPacketPool.Dequeue());
                }
            }
            SendStream();
        }

        public void SendStream()
        {
            Socket.BeginSend(Stream.GetBuffer(), (int)Stream.Position, (int)(Stream.Length - Stream.Position), SocketFlags.None, SendCallback, Socket);
        }

        //有可能一次发送没有把流中的数据发送完
        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (!socket.Connected)
            {
                return;
            }
            int bytesSent = socket.EndSend(ar);
            Stream.Position += bytesSent;
            if (Stream.Position < Stream.Length)
            {
                SendStream();
                return;
            }
            else
            {
                Stream.Position = 0;
            }
        }

        //预先开辟一个流用来中转数据=>组合到目标流
        private MemoryStream CachedStream = new MemoryStream(1024 * 8);
        public void SendPacket(Packet packet)
        {
            CachedStream.Position = 0L;
            CachedStream.SetLength(0);
            //获得ID字节数组
            byte[] idBytes = BitConverter.GetBytes(packet.ID);
            //获得消息
            IMessage message = packet.message;
            //获得消息长度字节数组
            int messageSize = message.CalculateSize() + 6;

            byte[] messageSizeBytes = BitConverter.GetBytes(messageSize);
            //将大消息总长度写入缓存
            CachedStream.Write(messageSizeBytes, 0, messageSizeBytes.Length);
            //将消息内容写入缓存流
            message.WriteTo(CachedStream);
            CachedStream.WriteTo(Stream);
        }
    }
}
