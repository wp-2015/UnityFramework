using Netwrok;
using Protocol;

public class ChatProxy : Proxy
{

    public void SendInt(int x)
    {
        var packet = CreatePacket<C2STestMessage>(MSGTYPE.MtC2STestMessage);
        packet.msg.ID = x;
        SendSocket(packet);
    }
}
