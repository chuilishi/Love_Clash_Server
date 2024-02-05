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
    public static Logger logger = new Logger();
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();
    public static JsonSerializerSettings jsonSetting = new JsonSerializerSettings
    {
        // NullValueHandling = NullValueHandling.Ignore
    };
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
            string s = await NetworkUtility.ReadAsync(client);
            var operation = JsonConvert.DeserializeObject<Operation>(s,jsonSetting);
            //尝试连接
            if (operation.operationType == OperationType.TryConnectRoom)
            {
                int roomInt = int.Parse(operation.extraMessage);
                Logger.Log("房间号为: "+roomInt+ "client类型为" + Enum.GetName(clientType));
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