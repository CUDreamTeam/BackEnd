using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerHandler
{
    private Server testServer = null;

    [SerializeField] private ServerGameHandler serverGameHandler = null;

    [SerializeField] private LobbyUIHandler lobbyUI = null;

    byte connected = 0;
    Dictionary<int, Player> connectedPlayers = new Dictionary<int, Player>();

    public static ServerHandler instance = null;

    public bool Connected
    {
        get
        {
            //Debug.Log((testServer != null) + " : " + (testServer.listener != null));
            return testServer != null && testServer.listener != null;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        testServer = new Server(50200, NewConnection);
        CreateLobby();
    }

    public void CreateLobby()
    {
        lobbyUI.gameObject.SetActive(true);
    }

    public void NewConnection(Socket socket)
    {
//        Debug.Log("Handling new connection");
        connectedPlayers.Add(connected, new Player("Not Set", connected, socket));
        byte[] temp = Encoding.ASCII.GetBytes("AAA");
        temp[2] = connected;
        SendMessage(socket, PackageMessage(temp));
        connected++;

        //lobbyUI.SetContent(connectedPlayers);

        Thread thread = new Thread(new ThreadStart(delegate { HandleReceive(socket, connected - 1); }));
        thread.Start();
    }

    private byte[] PackageMessage(string message)
    {
        byte[] toReturn = new byte[3];
        //Start of message marker
        toReturn[0] = 0xFF;
        toReturn[1] = 0xFF;
        //Length of message
        toReturn[2] = (byte)message.Length;

        return CombineByteArrays(toReturn, Encoding.ASCII.GetBytes(message));
    }
    private byte[] PackageMessage(byte[] message)
    {
        byte[] toReturn = new byte[3];
        //Start of message marker
        toReturn[0] = 0xFF;
        toReturn[1] = 0xFF;
        //Length of message
        toReturn[2] = (byte)message.Length;

        return CombineByteArrays(toReturn, message);
    }

    private byte[] CombineByteArrays(byte[] a, byte[] b)
    {
        byte[] toReturn = new byte[a.Length + b.Length];
        for (int i = 0; i < a.Length; i++) toReturn[i] = a[i];
        for (int i = 0; i < b.Length; i++) toReturn[i + a.Length] = b[i];
        return toReturn;
    }

    private byte[] RemoveArrayStart(byte[] start, int count)
    {
        byte[] toReturn = new byte[start.Length - count];
        for (int i = count; i < start.Length; i++) toReturn[i - count] = start[i];
        return toReturn;
    }

    public void SendMessage(Socket socket, byte[] message)
    {
        socket.Send(message);
    }

    private void HandleReceive(Socket socket, int playerIndex)
    {
        Debug.Log("Handling receive");
        while (socket.Connected)
        {
            byte[] message = new byte[1024];
            int received = 0;
            while (socket.Available > 0)
            {
                byte[] temp = new byte[1024];
                int count = socket.Receive(temp);
//                Debug.Log("Received " + count + " bytes");
                //for (int i = 0; i < count; i++) Debug.Log(temp[i]);

                if (received == 0)
                {
                    if (count >= 4)
                    {
                        if (count == temp[2] + 4)
                        {
                            TryParse(temp, temp[2]);
                        }
                        else if (count > temp[2] + 4)
                        {
                            Debug.Log("Received more bytes: " + count + " > " + (temp[2] + 4));
                        }
                        else
                        {
                            message = temp;
                            received = count;
                        }
                    }
                    else
                    {
                        message = temp;
                        received = count;
                    }
                }
                else
                {
                    CombineByteArrays(message, temp);
                    received += count;
                    if (received >= 3)
                    {
                        if (received == message[2] + 4)
                        {
                            TryParse(message, message[2]);
                            received = 0;
                            message = new byte[1024];
                        }
                        else if (count > message[2] + 4)
                        {
                            Debug.Log("Rebuilt to be greater than count");
                        }
                    }
                }
            }
        }
        Debug.Log("Player " + playerIndex + " has disconnected");
    }

    //Count does not include player index
    private void TryParse(byte[] message, int count)
    {
        if (message[0] == message[1] && message[0] == 0xFF)
        {
            byte[] temp = new byte[count + 1];
            for (int i = 0; i < count + 1; i++)
            {
                temp[i] = message[i + 3];
            }
            ParseData(temp);
        }
        else
        {
            Debug.Log("Message has bad start");
        }
    }

    private void ParseData(byte[] message)
    {
//        Debug.Log("Server is parsing data from " + message[0]);
        switch (message[1])
        {
            case (byte)'A':
                //Debug.Log("Received message: " + message.Remove(0));
                ParseStartData(message);
                break;
            case (byte)'B':
                Debug.Log("Parsing socket message");
                ParseSocketMessage(message);
                break;
            case (byte)'C':

                break;
            default:
                Debug.Log("Received bad message marker: " + message[1]);
                break;
        }
    }

    private void ParseStartData(byte[] message)
    {
//        Debug.Log("Server is parsing start data");
        int playerCode = message[0];
        byte[] temp = null;

        switch (message[2])
        {
            case (byte)'A':
                Debug.Log(message);
                SendMessage(connectedPlayers[playerCode].PlayerSocket, PackageMessage("AA"));
                break;
            case (byte)'B':
                //Debug.Log("Received message from " + playerCode);
                //Debug.Log("Received username: " + Encoding.ASCII.GetString(temp));
                temp = RemoveArrayStart(message, 3);
                Debug.Log("Setting username of " + playerCode + " to " + Encoding.ASCII.GetString(temp));
                connectedPlayers[playerCode].Username = Encoding.ASCII.GetString(temp);
                lobbyUI.SetContent(connectedPlayers);
                SendCurrentUsers();
                break;
            case (byte)'C':
                break;
            default:
                break;
        }
    }

    private void ParseSocketMessage(byte[] message)
    {
        switch (message[2])
        {
            case (byte)'A':
                Debug.Log("Disconneecting " + message[0]);
                OnPlayerDisconnect(connectedPlayers[message[0]]);
                break;
            default:
                Debug.Log("Received bad socket message marker: " + message[0]);
                break;
        }
    }

    public void ParseCreateMessage(byte[] message)
    {
        switch (message[2])
        {
            //Player is requesting player data
            case (byte)'A':
                break;
            default:
                break;
        }
    }

    private void OnPlayerDisconnect(Player p)
    {
        p.PlayerSocket.Disconnect(false);
        connectedPlayers.Remove(p.PlayerCode);
        SendCurrentUsers();
        lobbyUI.SetContent(connectedPlayers);
    }

    private void SendCurrentUsers()
    {
        byte[] message = Encoding.ASCII.GetBytes("AB");
        byte[] temp = new byte[1] { connected };

        //message = CombineByteArrays(message, temp);

        foreach (Player p in connectedPlayers.Values)
        {
            temp = new byte[1] { (byte)p.PlayerCode };
            message = CombineByteArrays(message, temp);
            temp = Encoding.ASCII.GetBytes(p.Username + ':');
            message = CombineByteArrays(message, temp);
        }

        foreach (Player p in connectedPlayers.Values)
        {
            if (p.PlayerSocket != null && p.PlayerSocket.Connected)
            {
                //Send message to clear connected users
                SendMessage(p.PlayerSocket, PackageMessage(message));
//                Debug.Log("Sending connected to player");
            }
        }
    }

    public void StartGame()
    {
        StopAcceptingConnections();
        lobbyUI.gameObject.SetActive(false);
        serverGameHandler = Instantiate(serverGameHandler.gameObject).GetComponent<ServerGameHandler>();
        serverGameHandler.serverHandler = this;
        serverGameHandler.StartGame(connectedPlayers);
    }

    public void StopAcceptingConnections()
    {
        testServer.StopAccept = true;
    }

    public void BroadcastHexMapData(int cellCountX, int cellCountZ, int state)
    {
        byte[] startMessage = Encoding.ASCII.GetBytes("CA");
        byte[] xSizeMessage = Encoding.ASCII.GetBytes("CB" + cellCountX);
        byte[] zSizeMessage = Encoding.ASCII.GetBytes("CC" + cellCountX);
        byte[] stateMessage = Encoding.ASCII.GetBytes("CD" + state);
        byte[] buildMessage = Encoding.ASCII.GetBytes("CE");

        BroadcastMessage(PackageMessage(startMessage));
        BroadcastMessage(PackageMessage(xSizeMessage));
        BroadcastMessage(PackageMessage(zSizeMessage));
        BroadcastMessage(PackageMessage(stateMessage));
        BroadcastMessage(PackageMessage(buildMessage));
    }

    public void SendPlayerStartPosition(int playerIndex, HexCoordinates coords)
    {
        byte[] message = new byte[3] { (byte)'C', (byte)'F', (byte)playerIndex};
        message = CombineByteArrays(message, Encoding.ASCII.GetBytes("" + coords.X));
        message = CombineByteArrays(message, Encoding.ASCII.GetBytes("," + coords.Z));
        BroadcastMessage(PackageMessage(message));
    }

    public void SendNewUnit(int playerIndex, HexCoordinates coords, int unitType)
    {
        byte[] message = new byte[3] { (byte)'C', (byte)'G', (byte)playerIndex };
        message = CombineByteArrays(message, Encoding.ASCII.GetBytes("" + coords.X));
        message = CombineByteArrays(message, Encoding.ASCII.GetBytes("," + coords.Z));
        message = CombineByteArrays(message, Encoding.ASCII.GetBytes("," + unitType));
        BroadcastMessage(PackageMessage(message));
    }

    private void BroadcastMessage(byte[] message)
    {
        Debug.Log("Sending message to " + connectedPlayers.Count + " players");
        foreach (Player p in connectedPlayers.Values)
        {
            Debug.Log("Sent " + p.PlayerSocket.Send(message) + " bytes");
        }
    }

    public void Disconnect()
    {
        if (testServer != null)
        {
            byte[] message = Encoding.ASCII.GetBytes("BA");
            foreach (Player p in connectedPlayers.Values)
            {
                p.PlayerSocket.Send(PackageMessage(message));
            }
            Debug.Log("Sent socket message");
            testServer.Disconnect();
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
