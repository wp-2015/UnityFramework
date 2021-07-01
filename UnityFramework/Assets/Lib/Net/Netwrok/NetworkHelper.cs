using Google.Protobuf;
using System;
using System.IO;

namespace Netwrok
{
    public static class NetworkHelper
    {
        private static MemoryStream CachedStream = new MemoryStream(1024 * 8);
        public static void SendPacket(Packet packet)
        {
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
        }
    }
}
