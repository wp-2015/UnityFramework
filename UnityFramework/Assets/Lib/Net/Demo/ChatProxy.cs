using Google.Protobuf;
using Netwrok;
using Protocol;
using UnityEngine;

public class ChatProxy : Proxy<ChatProxy>
{
    public ChatProxy()
    {
        AddServerListener(MSGTYPE.MtS2CTestMessage, ReciveTest);
    }

    public void SendInt(int x)
    {
        var packet = CreatePacket<C2STestMessage>(MSGTYPE.MtC2STestMessage);
        packet.msg.ID = x;
        SendSocket(packet);
    }

    public void ReciveTest(IMessage message)
    {
        S2CTestMessage msg = message as S2CTestMessage;
        Debug.LogError("sz : " + msg.Sz);
        Debug.LogError("longID : " + msg.LongID);
    }
}
