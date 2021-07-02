using Netwrok;
using Protocol;

public class ChatProxy : Proxy
{

    public void SendInt(int x)
    {
        var packet = CreatePacket<C2STestMessage>(MSGTYPE.C2StestMessage);
        packet.msg.ID = x;
        SendSocket(packet);
    }
}
