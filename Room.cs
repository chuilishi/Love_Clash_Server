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
    //0是Player1,1是Player2
    public int objectCount = 2;
    public Room(int roomId)
    {
        this.roomId = roomId;
    }
    
    private async Task MessageHandler(Operation operation)
    {
        if (operation.playerEnum == PlayerEnum.Player1)
        {
            await NetworkUtility.BroadCast(operation,client1Sender,client2Receiver);
        }
        else
        {
            await NetworkUtility.BroadCast(operation,client2Sender,client1Receiver);
        }
    }

    #region Room基本设施

    public void AddPlayer(TcpClient client,ClientType clientType)
    {
        string s = "";
        if (clientType == ClientType.Sender)
        {
            if (client1Sender == null)
            {
                client1Sender = client;
                s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player1, extraMessage: roomId.ToString()),Network.jsonSetting);
            }
            else if (client2Sender == null)
            {
                client2Sender = client;
                s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player2, extraMessage: roomId.ToString()),Network.jsonSetting);
            }
            else
            {
                s = JsonConvert.SerializeObject(new Operation(OperationType.Error),Network.jsonSetting);
            }
        }
        else if (clientType == ClientType.Receiver)
        {
            if (client1Receiver == null)
            {
                client1Receiver = client;
                s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player1, extraMessage: roomId.ToString()),Network.jsonSetting);
            }
            else if (client2Receiver == null)
            {
                client2Receiver = client;
                s = JsonConvert.SerializeObject(new Operation(OperationType.TryConnectRoom,
                    playerEnum: PlayerEnum.Player2, extraMessage: roomId.ToString()),Network.jsonSetting);
            }
            else
            {
                s = JsonConvert.SerializeObject(new Operation(OperationType.Error),Network.jsonSetting);
            }
        }
        NetworkUtility.WriteAsync(client, Encoding.ASCII.GetBytes(s));
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
        Console.WriteLine("Start");
        NetworkUtility.BroadCast(new Operation(OperationType.Init),client1Receiver,client2Receiver);
        //对每个sender回应另一个sender的Init
        var task1 = NetworkUtility.ReadAsync(client1Sender);
        var task2 = NetworkUtility.ReadAsync(client2Sender);
        await Task.WhenAll(task1,task2);
        NetworkUtility.WriteAsync(client1Sender,task2.Result);
        NetworkUtility.WriteAsync(client2Sender,task1.Result);
        
        MessageHandler(client1Sender,PlayerEnum.Player1);
        MessageHandler(client2Sender,PlayerEnum.Player2);
    }
    
    public async Task MessageHandler(TcpClient client,PlayerEnum playerEnum)
    {
        while (true)
        {
            try
            {
                string resp = await NetworkUtility.ReadAsync(client);
                if (string.IsNullOrEmpty(resp))
                {
                    if (playerEnum == PlayerEnum.Player1)
                    {
                        Logger.Log("Player1退出");
                    }
                    else if (playerEnum == PlayerEnum.Player2)
                    {
                        Logger.Log("Player2退出");
                    }
                    client1Receiver.Close();
                    client1Sender.Close();
                    client2Receiver.Close();
                    client2Sender.Close();
                    Network.rooms.Remove(roomId);
                    break;
                }
                Logger.Log("收到的消息: "+resp);
                Operation operation = JsonConvert.DeserializeObject<Operation>(resp,Network.jsonSetting);
                MessageHandler(operation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
    private void CreateObject(Operation operation)
    {
        var networkObject = JsonConvert.DeserializeObject<NetworkObject>(operation.baseNetworkObjectJson);
        networkObject.networkId = objectCount;
        operation.baseNetworkObjectJson = JsonConvert.SerializeObject(networkObject,Network.jsonSetting);
        objectCount++;
        var s = JsonConvert.SerializeObject(operation,Network.jsonSetting);
        Console.WriteLine("发送的消息: "+s);
        if (operation.playerEnum == PlayerEnum.Player1)
        {
            NetworkUtility.WriteAsync(client1Sender,s);
            NetworkUtility.WriteAsync(client2Receiver,s);
        }
        else if (operation.playerEnum == PlayerEnum.Player2)
        {
            NetworkUtility.WriteAsync(client2Sender,s);
            NetworkUtility.WriteAsync(client1Receiver,s);
        }
    }

    #endregion
    
}