using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using loveclash_server;
using loveclash_server.UnityPart;
using Newtonsoft.Json;

public class Network()
{
    public static int receiverPort = 7777;
    public static int senderPort = 7778;
    public static string ipaddress = "127.0.0.1";
    public static TcpListener receiverListener;
    public static TcpListener senderListener;
    public Dictionary<int, Room> rooms = new Dictionary<int, Room>();
    public async Task Start(){
        receiverListener = new TcpListener(new IPEndPoint(IPAddress.Parse(ipaddress),receiverPort));
        senderListener = new TcpListener(new IPEndPoint(IPAddress.Parse(ipaddress),senderPort));
        Listener(receiverListener, ClientType.Receiver);
        Listener(senderListener, ClientType.Sender);
        //prevent the program to exit
        Console.ReadLine();
    }

    public async Task Listener(TcpListener listener,ClientType clientType)
    {
        listener.Start();
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            ClientHandler(client,clientType);
        }
    }
    public async void ClientHandler(TcpClient client,ClientType clientType)
    {
        try
        {
            #region 首次连接
            var stream = client.GetStream();
            byte[] buffer = new byte[4096];
            int contentSize = await stream.ReadAsync(buffer,0,buffer.Length);
            Console.WriteLine("连接成功");
            var formatted = new byte[contentSize];
            Array.Copy(buffer,formatted,contentSize);
            var s = Encoding.ASCII.GetString(formatted);
            Console.WriteLine(s);
            var operation = JsonConvert.DeserializeObject<Operation>(s);
            //尝试连接
            if (operation.operationType == OperationType.TryConnectRoom)
            {
                int roomInt = int.Parse(operation.extraMessage);
                Console.WriteLine("房间号为: "+roomInt+ "client类型为" + Enum.GetName(clientType));
                if (!rooms.ContainsKey(roomInt))rooms[roomInt] = new Room(roomInt);
                rooms[roomInt].AddPlayer(client,clientType);
            }
            #endregion
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("断开连接");
        }
    }
}

public enum ClientType
{
    Sender,
    Receiver
}