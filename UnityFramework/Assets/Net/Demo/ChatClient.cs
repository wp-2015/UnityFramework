using NetCoreServer;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

 class ClientEntity : TcpClient
{
    public ClientEntity(string address, int port) : base(address, port) { }
    private bool _stop;

    protected override void OnConnected()
    {
        Debug.LogError("Chat TCP client connected a new session with Id" + Id);
    }

    protected override void OnDisconnected()
    {
        Debug.LogError($"Chat TCP client disconnected a session with Id " + Id);

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            ConnectAsync();
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        Debug.LogError("++++++++" + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
    }
}

public class ChatClient : MonoBehaviour
{
    private void Start()
    {
        string address = "127.0.0.1";
        int port = 8999;

        var client = new ClientEntity(address, port);
        Debug.LogError("++++¿ªÊ¼Á´½Ó");
        client.ConnectAsync();
    }
}
