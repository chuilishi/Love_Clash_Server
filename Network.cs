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
    public static int receiverPort = 7788;
    public static int senderPort = 7799;
    public static string ipaddress = "127.0.0.1";
    public static TcpListener receiverListener;
    public static TcpListener senderListener;
    public static Logger logger = new Logger();
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();
    public static JsonSerializerSettings jsonSetting = new JsonSerializerSettings
    {
        // NullValueHandling = NullValueHandling.Ignore
    };
    public void Start(){
        Console.WriteLine("networkStart");
        receiverListener = new TcpListener(IPAddress.Parse(ipaddress), receiverPort);
        Console.WriteLine("receiveListener start");
        senderListener = new TcpListener(new IPEndPoint(IPAddress.Parse(ipaddress),senderPort));
        Console.WriteLine("senderListener start");
        Listener(receiverListener, ClientType.Receiver);
        Listener(senderListener, ClientType.Sender);
        //prevent the program to exit
        Console.ReadLine();
    }
    public async Task Listener(TcpListener listener,ClientType clientType)
    {
        Console.WriteLine("listener start");
        listener.Start();
        while (true)
        {
            Console.WriteLine("kaishi");
            var client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("有连接");
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