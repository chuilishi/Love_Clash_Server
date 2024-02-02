namespace loveclash_server.UnityPart;

/// <summary>
/// operation 中的所有东西都能
/// </summary>
/// 
[Serializable]
public class Operation
{
    public OperationType operationType = OperationType.Error;
    public int operationId = 0;
    public PlayerEnum playerEnum;
    public NetworkObject baseNetworkObject;
    public List<NetworkObject> targetNetworkObjects;
    /// <summary>
    /// 一些额外的附加信息 比如connect时对方的用户名
    /// </summary>
    public string extraMessage;
    public Operation(OperationType operationType = OperationType.Error,PlayerEnum playerEnum = PlayerEnum.NotReady,string extraMessage=null,NetworkObject baseNetworkObject = null,List<NetworkObject> targetNetworkObjects=null)
    {
        this.operationType = operationType;
        this.playerEnum = playerEnum;
        this.baseNetworkObject = baseNetworkObject;
        this.targetNetworkObjects = targetNetworkObjects;
        this.extraMessage = extraMessage;
    }
}
public enum PlayerEnum
{
    Player1=1,
    Player2=2,
    NotReady=3
}
public enum OperationType
{
    Error,
    Init,
    /// <summary>
    /// 申请一个NetworkObject的唯一id
    /// </summary>
    GetObjectId,
    Card,
    Skill,
    /// <summary>
    /// 尝试连接,无房间就创建一个
    /// </summary>
    TryConnectRoom,
    EndTurn,
}
public enum RequestType
{
    Request,
    BroadCast
}