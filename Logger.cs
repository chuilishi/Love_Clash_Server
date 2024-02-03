using System.Text;
using loveclash_server.UnityPart;

namespace loveclash_server;

public class Logger
{
    public static FileStream fileStream;
    public Logger()
    {
        //open Log.txt in current directory as a filestream
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Log.txt");
        fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
    }
    public static void Log(string value)
    {
        value += "\n";
        var bytes = Encoding.ASCII.GetBytes(value);
        fileStream.WriteAsync(bytes,0,bytes.Length).GetAwaiter().OnCompleted((() =>
        {
            fileStream.Flush();
        }));
        Console.WriteLine(value);
    }
}