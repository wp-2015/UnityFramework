package Demo

import (
	protocol "GameServer/src/PB"
	"GameServer/ziface"
	"GameServer/zlog"
	"fmt"
	"github.com/golang/protobuf/proto"
)

func init() {
}


func AddHandle(server ziface.IServer){
	AddFuncHandle(server, protocol.MSGTYPE_Mt_C2S_TestMessage, C2STestMessage)
}

func AddFuncHandle(server ziface.IServer, id protocol.MSGTYPE, funcHandle func(request ziface.IRequest)){
	code := uint16(id)
	server.AddFuncRouter(code, funcHandle)
}

func C2STestMessage(request ziface.IRequest)  {
	c2sTestMessage := &protocol.C2STestMessage{}

	err := proto.Unmarshal(request.GetData(), c2sTestMessage)
	if nil != err{
		zlog.Error("error")
	}
	fmt.Println("id = ", c2sTestMessage.ID)

	//s2dPacket := new(protocol.S2CTestMessage)  
	//
	//s2dPacket.LongID = 100
	//s2dPacket.Sz = "1234654324345"
	//
	//data, err := proto.Marshal(s2dPacket)
	//code := uint16(protocol.MSGTYPE_Mt_S2C_TestMessage)
	//request.GetConnection().SendBuffMsg(code, data)
}

