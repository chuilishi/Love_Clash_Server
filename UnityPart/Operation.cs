using Newtonsoft.Json;

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
    /// <summary>
    /// 因为JsonUtility的嵌套序列化会有一些问题, 使用额外的string存储
    /// </summary>
    public string baseNetworkObjectJson;
    public List<string> targetNetworkObjectsJson;
    /// <summary>
    /// 一些额外的附加信息 比如connect时对方的用户名
    /// </summary>
    public string extraMessage;
    public Operation(OperationType operationType = OperationType.Error,PlayerEnum playerEnum = PlayerEnum.NotReady,string extraMessage=null)
    {
        this.operationType = operationType;
        this.playerEnum = playerEnum;
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
    CreateObject,
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