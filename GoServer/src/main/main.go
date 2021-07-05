package main

import (
	"GameServer/src/Demo"
	"GameServer/ziface"
	"GameServer/znet"
	"fmt"
)

type TestRouter struct {
	znet.BaseRouter
}

func (this *TestRouter) Handle(request ziface.IRequest)  {
	fmt.Println("TestRouter Handle")
}

func main() {
	fmt.Println("asdasd")
	s := znet.NewServer()
	Demo.AddHandle(s)
	//3 开启服务
	s.Serve()
}