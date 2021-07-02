using Protocol;
using System;
using System.Collections.Generic;

namespace Netwrok
{
    public static class ServerListener
    {
        public static Dictionary<MSGTYPE, EventHandler> dicMsgHandle = new Dictionary<MSGTYPE, EventHandler>();

        public static void Add(MSGTYPE type, EventHandler handler)
        {
            dicMsgHandle[type] = handler;
        }

        public static void Handler(MSGTYPE type)
        {

        }
    }
}
