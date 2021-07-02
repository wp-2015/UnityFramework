using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;

namespace Netwrok
{
    public static class ServerListener
    {
        public static Dictionary<MSGTYPE, List<Action<IMessage>>> dicMsgHandle = new Dictionary<MSGTYPE, List<Action<IMessage>>>();

        public static void AddHandle(MSGTYPE type, Action<IMessage> handler)
        {
            if(!dicMsgHandle.TryGetValue(type, out List<Action<IMessage>> handles))
            {
                handles = new List<Action<IMessage>>();
                dicMsgHandle[type] = handles;
            }
            handles.Add(handler);
        }

        public static void Handler(MSGTYPE type, byte[] messageByte)
        {
            Type messageType = MessageTypeMap.messageTypeDic[type];
            if(null != messageType)
            {
                IMessage message = Activator.CreateInstance(messageType) as IMessage;
                message.MergeFrom(messageByte);
                if(dicMsgHandle.TryGetValue(type, out List<Action<IMessage>> handles))
                {
                    for(int i = 0; i < handles.Count; i++)
                    {
                        var handle = handles[i];
                        if(null != handle)
                            handle(message);
                    }
                }
            }
        }
    }
}
