using System.Net.Sockets;
using System.Text;
using loveclash_server.UnityPart;
using Newtonsoft.Json;
namespace loveclash_server;

public class Room
{
    //TODO 断线的处理
    public TcpClient client1Sender;
    public TcpClient client1Receiver;
    public TcpClient client2Sender;
    public TcpClient client2Receiver;
    public int roomId;
    public int objectCount = 0;
    public Room(int roomId)
    {
        this.roomId = roomId;
    }
    public void AddPlayer(TcpClient client,ClientType clientType)
    {
        if (clientType == ClientType.Sender)
        {
            if (client1Sender == null)
            {
                client1Sender = client;
                string s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player1, extraMessage: roomId.ToString()),Network.jsonSetting);
                client.GetStream().Write(Encoding.ASCII.GetBytes(s));
            }
            else if (client2Sender == null)
            {
                client2Sender = client;
                string s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player2, extraMessage: roomId.ToString()),Network.jsonSetting);
                client.GetStream().Write(Encoding.ASCII.GetBytes(s));
            }
            else
            {
                string s = JsonConvert.SerializeObject(new Operation(OperationType.Error),Network.jsonSetting);
                client.GetStream().Write(Encoding.ASCII.GetBytes(s));
            }
        }
        else if (clientType == ClientType.Receiver)
        {
            if (client1Receiver == null)
            {
                client1Receiver = client;
                string s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player1, extraMessage: roomId.ToString()),Network.jsonSetting);
                client.GetStream().Write(Encoding.ASCII.GetBytes(s));
            }
            else if (client2Receiver == null)
            {
                client2Receiver = client;
                string s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player2, extraMessage: roomId.ToString()),Network.jsonSetting);
                client.GetStream().Write(Encoding.ASCII.GetBytes(s));
            }
            else
            {
                string s = JsonConvert.SerializeObject(new Operation(OperationType.Error),Network.jsonSetting);
                client.GetStream().Write(Encoding.ASCII.GetBytes(s));
            }
        }
        if (client1Receiver!=null && client1Sender!=null && client2Receiver!=null &&
            client2Sender!=null)
        {
            Start();
        }
    }

    public async void Start()
    {
        if(!client1Receiver.Connected||!client1Sender.Connected||!client2Receiver.Connected||!client2Sender.Connected)
            throw new Exception("有玩家未连接");
        //发一个Init代表开始
        BroadCast(new Operation(OperationType.Init));
        //对每个sender回应另一个sender的Init
        var task1 = ReceiveAsync(client1Sender);
        var task2 = ReceiveAsync(client2Sender);
        await Task.WhenAll(task1,task2);
        client1Sender.GetStream().WriteAsync(Encoding.ASCII.GetBytes(task2.Result));
        client2Sender.GetStream().WriteAsync(Encoding.ASCII.GetBytes(task1.Result));
        MessageHandler(client1Sender);
        MessageHandler(client2Sender);
    }
    
    private static async Task<string> ReceiveAsync(TcpClient client)
    {
        var buffer = new byte[4096];
        client.GetStream().ReadTimeout = 10000;
        int size = 0;
        size = await client.GetStream().ReadAsync(buffer,0,buffer.Length);
        var formatted = new byte[size];
        Array.Copy(buffer,formatted,size);
        string s = Encoding.ASCII.GetString(formatted);
        Logger.Log("收到的消息: "+s);
        return s;
    }
    public void BroadCast(byte[] bytes)
    {
        client1Receiver.GetStream().WriteAsync(bytes);
        client2Receiver.GetStream().WriteAsync(bytes);
    }
    public void BroadCast(string s)
    {
        BroadCast(Encoding.ASCII.GetBytes(s));
    }
    public void BroadCast(Operation operation)
    {
        BroadCast(JsonConvert.SerializeObject(operation,Network.jsonSetting));
    }
    public async void MessageHandler(TcpClient client)
    {
        while (true)
        {
            var resp = await ReceiveAsync(client);
            Logger.Log("收到的消息: "+resp);
            var operation = JsonConvert.DeserializeObject<Operation>(resp,Network.jsonSetting);
            if (operation.operationType == OperationType.CreateObject)
            {
                CreateObject(operation);
            }
            else
            {
                BroadCast(resp);
            }
        };
    }
    
    private void CreateObject(Operation operation)
    {
        var networkObject = JsonConvert.DeserializeObject<NetworkObject>(operation.baseNetworkObjectJson);
        networkObject.networkId = objectCount;
        objectCount++;
        var s = JsonConvert.SerializeObject(operation,Network.jsonSetting);
        if (operation.playerEnum == PlayerEnum.Player1)
        {
            client1Sender.GetStream().WriteAsync(Encoding.ASCII.GetBytes(s));
            client2Receiver.GetStream().WriteAsync(Encoding.ASCII.GetBytes(s));
        }
        else if (operation.playerEnum == PlayerEnum.Player2)
        {
            client2Sender.GetStream().WriteAsync(Encoding.ASCII.GetBytes(s));
            client1Receiver.GetStream().WriteAsync(Encoding.ASCII.GetBytes(s));
        }
    }
    
}