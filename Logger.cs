using System.Text;
using loveclash_server.UnityPart;

namespace loveclash_server;

public class Logger
{
    public static FileStream fileStream;
    public Logger()
    {
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "logs")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
        }
        string path = "";
        while (true)
        {
            int logCount = 0;
            path = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log_" + logCount + ".txt");
            if (Path.Exists(path))break;
            logCount++;
        }
        fileStream = new FileStream(path,FileMode.OpenOrCreate);
    }
    public static void Log(string value)
    {
        value += "\n";
        var bytes = Encoding.UTF8.GetBytes(value);
        fileStream.WriteAsync(bytes,0,bytes.Length).GetAwaiter().OnCompleted((() =>
        {
            fileStream.Flush();
        }));
    }
}