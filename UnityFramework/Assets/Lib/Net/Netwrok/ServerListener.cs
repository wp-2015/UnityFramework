using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;

namespace Netwrok
{
    public static class ServerListener
    {
        public static Dictionary<MSGTYPE, Action<IMessage>> dicMsgHandle = new Dictionary<MSGTYPE, Action<IMessage>>();

        public static void Add(MSGTYPE type, Action<IMessage> handler)
        {
            dicMsgHandle[type] = handler;
        }

        public static void Handler(MSGTYPE type, byte[] messageByte)
        {
            Type messageType = MessageTypeMap.messageTypeDic[type];
            if(null != messageType)
            {
                IMessage message = Activator.CreateInstance(messageType) as IMessage;
                message.MergeFrom(messageByte);
                if(dicMsgHandle.TryGetValue(type, out Action<IMessage> handle))
                {
                    handle(message);
                }
            }
        }
    }
}
