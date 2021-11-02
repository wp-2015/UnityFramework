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
using UnityEngine;

namespace Netwrok
{
    //TODO:取消掉中间Stream，但是要保证发送的时候不会往Stream中写入,需要做实验是否Socket.BeginSend可以在一帧内执行
    //TODO:确认Socket.BeginSend执行后Stream的变化包括Length和Position
    public class NetworkChannel
    {
        public struct MessageWaitHandleInfo
        {
            public int id;
            public byte[] msg;
        }
        //Socket
        protected Socket Socket;
        //默认字节流长度
        private const int DefaultBufferLength = 1024;
        private Action closeCB;

        const int HeadLength = 4;

        /***************************************发送使用**********************************/
        //Packet缓存池
        private Queue<Packet> sendPacketPool = new Queue<Packet>();
        //将要发送的数据放入流
        private MemoryStream memoryStreamSend = new MemoryStream(DefaultBufferLength);

        /***************************************接收使用**********************************/
        private MemoryStream memoryStreamReceive = new MemoryStream(DefaultBufferLength);

        public NetworkChannel(IPAddress ipAddress, int port, Action closeCB)
        {
            Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.BeginConnect(ipAddress, port, ConnectCB, null);
        }

        private void ConnectCB(IAsyncResult ar)
        {
            //ReceiveAsync();//异步
            //发送需要用到的初始化
            memoryStreamSend.Position = 0;
            //接收需要用到的初始化
            memoryStreamReceive.Position = 0;
            bIsReceiveHead = true;
            memoryStreamReceive.SetLength(HeadLength);
            ReceiveAsync();
        }

        public void Send(Packet packet)
        {
            sendPacketPool.Enqueue(packet);
        }

        public void Update()
        {
            if (null == Socket || !Socket.Connected)
            {
                Close();
                closeCB?.Invoke();
            }
            while (sendPacketPool.Count > 0)
            {
                if (memoryStreamSend.Length > 0)
                    break;
                lock (sendPacketPool)
                {
                    SendPacket(sendPacketPool.Dequeue());
                    memoryStreamSend.Position = 0;
                    SendStream();
                }
                //ReceiveAsync();//同步
            }
            while(lMessageWait.Count > 0)
            {
                var info = lMessageWait.Dequeue();
                ServerListener.Handler((MSGTYPE)info.id, info.msg);
            }
        }
        #region 发送
        /**********************************************************************发送*********************************************************/
        public void SendStream()
        {
            Socket.BeginSend(memoryStreamSend.GetBuffer(), 
                (int)memoryStreamSend.Position, 
                (int)(memoryStreamSend.Length - memoryStreamSend.Position), 
                SocketFlags.None, 
                SendCallback, 
                Socket);
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
                memoryStreamSend.SetLength(0);
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
            int messageSize = message.CalculateSize();
            byte[] messageSizeBytes = BitConverter.GetBytes(messageSize);
            //将消息总长度写入缓存
            CachedStream.Write(messageSizeBytes, 0, HeadLength);
            //将ID写入缓存
            CachedStream.Write(idBytes, 0, 2);
            //将消息内容写入缓存流
            message.WriteTo(CachedStream);
            CachedStream.WriteTo(memoryStreamSend);
        }
        /*************************************************************发送数据*************************************************************/
        #endregion

        #region 接收
        /*************************************************************接收数据*************************************************************/
        bool bIsReceiveHead = true;
        private void ReceiveAsync()
        {
            try
            {
                Socket.BeginReceive(memoryStreamReceive.GetBuffer(), 
                                        (int)memoryStreamReceive.Position,
                                        (int)(memoryStreamReceive.Length - memoryStreamReceive.Position),
                                        SocketFlags.None, ReceiveCallback, Socket);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
        //消息都需要主线程处理，因此设置堆栈，在update中处理
        Queue<MessageWaitHandleInfo> lMessageWait = new Queue<MessageWaitHandleInfo>();
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int bytesReceived = socket.EndReceive(ar);
            if (bytesReceived <= 0)
            {
                Debug.LogError("+++++ReceiveCallback bytesReceived 《= 0");
                //Close();
                //return;
            }
            memoryStreamReceive.Position += bytesReceived;

            Debug.LogError("Position+++++++++++" + memoryStreamReceive.Position + "    memoryStreamReceive.Length  " + memoryStreamReceive.Length);
            //信息在一次流传输中没有完成
            if (memoryStreamReceive.Position < memoryStreamReceive.Length)
            {
                ReceiveAsync();
                return;
            }

            memoryStreamReceive.Position = 0L;
            //一条协议的解析分为两次，一次为长度，一次为协议内容
            //长度解析
            if (bIsReceiveHead)
            {
                //大小
                byte[] messageSizeBytes = new byte[HeadLength];
                memoryStreamReceive.Read(messageSizeBytes, 0, HeadLength);
                int messageSize = BitConverter.ToInt32(messageSizeBytes, 0);
                Debug.LogError("messageSize+++++++++++" + messageSize);
                memoryStreamReceive.SetLength(messageSize + 2);
                bIsReceiveHead = false;
            }
            //内容解析
            else
            {
                //ID
                byte[] messageIdBytes = new byte[2];
                memoryStreamReceive.Read(messageIdBytes, 0, 2);
                for(int i = 0; i < 2; i++)
                {
                    Debug.LogError("666++++" + messageIdBytes[i]);
                }
                var id = BitConverter.ToInt16(messageIdBytes, 0);
                Debug.LogError("id+++++++++++" + id);
                //message
                byte[] messageBytes = new byte[memoryStreamReceive.Length - 2];
                memoryStreamReceive.Read(messageBytes, 0, messageBytes.Length);
                lMessageWait.Enqueue(new MessageWaitHandleInfo() { id = id, msg = messageBytes });
                //ServerListener.Handler((MSGTYPE)id, messageBytes);
                memoryStreamReceive.SetLength(HeadLength);
                bIsReceiveHead = true;
            }

            memoryStreamReceive.Position = 0L;
            ReceiveAsync();
        }

        //private bool ReceiveSync()
        //{

        //    memoryStreamReceive.Position = 0;
        //    //var bs = new byte[10000];
        //    //int bytesReceived = Socket.Receive(bs);


        //    int bytesReceived = Socket.Receive(memoryStreamReceive.GetBuffer(),
        //        (int)memoryStreamReceive.Position, (int)(memoryStreamReceive.Length - memoryStreamReceive.Position), SocketFlags.None);
        //    if (bytesReceived <= 0)
        //    {
        //        return false;
        //    }
        //    memoryStreamReceive.Position += bytesReceived;

        //    if (memoryStreamReceive.Position < memoryStreamReceive.Length)
        //    {
        //        return false;
        //    }
        //    //接收数据完毕
        //    memoryStreamReceive.Position = 0L;
        //    //大小
        //    byte[] messageSizeBytes = new byte[4];
        //    memoryStreamReceive.Read(messageSizeBytes, 0, 4);
        //    int messageSize = BitConverter.ToInt32(messageSizeBytes, 0);
        //    //ID
        //    byte[] messageIdBytes = new byte[2];
        //    memoryStreamReceive.Read(messageIdBytes, 0, 2);
        //    int id = BitConverter.ToInt32(messageIdBytes, 0);
        //    //message
        //    byte[] messageBytes = new byte[messageSize];
        //    memoryStreamReceive.Read(messageBytes, 0, messageBytes.Length);
        //    ServerListener.Handler((MSGTYPE)id, messageBytes);
        //    return true;
        //}

        /*************************************************************接收数据*************************************************************/
        #endregion

        #region Socket
        /*************************************************************Socket*************************************************************/
        private void Close()
        {
            if (null == Socket)
                return;

            if(Socket.Connected)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            memoryStreamReceive.Close();
            memoryStreamSend.Close();
            sendPacketPool.Clear();
        }
        /*************************************************************Socket*************************************************************/
        #endregion
    }
}
