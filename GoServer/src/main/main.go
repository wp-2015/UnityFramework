package main

import (
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

	//2 配置路由
	s.AddRouter(0, &TestRouter{})

	//3 开启服务
	s.Serve()
}