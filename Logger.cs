using loveclash_server.UnityPart;

namespace loveclash_server;

public class Logger
{
    public static void Log(Operation operation,Room room)
    {
        Console.WriteLine($"房间号{room.roomId}\n玩家{Enum.GetName(typeof(PlayerEnum),operation.playerEnum)}\n类型 {Enum.GetName(typeof(OperationType),operation.operationType)}");
    }
}