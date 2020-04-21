using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;


public class ServerPlayer
{
    public string Username { get; private set; } = string.Empty;
    public int PlayerCode { get; private set; } = 0;
    public Socket PlayerSocket { get; private set; } = null;
}
