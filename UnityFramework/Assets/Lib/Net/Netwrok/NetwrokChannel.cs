using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    //TODO:取消掉中间Stream，但是要保证发送的时候不会往Stream中写入,需要做实验是否Socket.BeginSend可以在一帧内执行
    //TODO:确认Socket.BeginSend执行后Stream的变化包括Length和Position
    public class NetworkChannel
    {
        //Socket
        protected Socket Socket;
        //默认字节流长度
        private const int DefaultBufferLength = 1024 * 64;

        /***************************************发送使用**********************************/
        //Packet缓存池
        private Queue<Packet> sendPacketPool = new Queue<Packet>();
        //将要发送的数据放入流
        private MemoryStream memoryStreamSend = new MemoryStream(DefaultBufferLength);

        /***************************************接收使用**********************************/
        private MemoryStream memoryStreamReceive = new MemoryStream(DefaultBufferLength);

        public NetworkChannel(IPAddress ipAddress, int port)
        {
            memoryStreamSend.Position = 0;
            Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
                    SendPacket(sendPacketPool.Dequeue());
                }
            }
            SendStream();
            ReceiveSync();
        }
        #region 发送
        /**********************************************************************发送*********************************************************/
        public void SendStream()
        {
            Socket.BeginSend(memoryStreamSend.GetBuffer(), (int)memoryStreamSend.Position, (int)(memoryStreamSend.Length - memoryStreamSend.Position), SocketFlags.None, SendCallback, Socket);
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
            memoryStreamSend.Position += bytesSent;
            if (memoryStreamSend.Position < memoryStreamSend.Length)
            {
                SendStream();
                return;
            }
            else
            {
                memoryStreamSend.Position = 0;
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
            //将消息总长度写入缓存
            CachedStream.Write(messageSizeBytes, 0, messageSizeBytes.Length);
            //将ID写入缓存
            CachedStream.Write(messageSizeBytes, 0, idBytes.Length);
            //将消息内容写入缓存流
            message.WriteTo(CachedStream);
            CachedStream.WriteTo(memoryStreamSend);
        }
        /*************************************************************发送数据*************************************************************/
        #endregion

        #region 接收
        /*************************************************************接收数据*************************************************************/
        private bool ReceiveSync()
        {
            int bytesReceived = Socket.Receive(memoryStreamReceive.GetBuffer(), 
                (int)memoryStreamReceive.Position, (int)(memoryStreamReceive.Length - memoryStreamReceive.Position), SocketFlags.None);
            if (bytesReceived <= 0)
            {
                return false;
            }
            memoryStreamReceive.Position += bytesReceived;

            if (memoryStreamReceive.Position < memoryStreamReceive.Length)
            {
                return false;
            }
            //接收数据完毕
            memoryStreamReceive.Position = 0L;
            //大小
            byte[] messageSizeBytes = new byte[4];
            memoryStreamReceive.Read(messageSizeBytes, 0, 4);
            int messageSize = BitConverter.ToInt32(messageSizeBytes, 0);
            //ID
            byte[] messageIdBytes = new byte[2];
            memoryStreamReceive.Read(messageIdBytes, 0, 2);
            int id = BitConverter.ToInt32(messageIdBytes, 0);
            //message
            byte[] messageBytes = new byte[messageSize - 6];
            memoryStreamReceive.Read(messageBytes, 0, messageBytes.Length);

            ServerListener.Handler((MSGTYPE)id, messageBytes);
            return true;
        }

        /*************************************************************接收数据*************************************************************/
        #endregion

        #region Socket
        /*************************************************************Socket*************************************************************/

        /*************************************************************Socket*************************************************************/
        #endregion
    }
}
