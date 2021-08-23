using System;
using System.Collections.Generic;
using Protocol;

/// <summary>
/// 所有需要发送和接收的协议都要在这个注册
/// </summary>
public static class MessageTypeMap
{
    static public Dictionary<MSGTYPE, Type> messageTypeDic = new Dictionary<MSGTYPE, Type>()
    {
		{MSGTYPE.MtS2CTestMessage, typeof(S2CTestMessage)},

	};
}