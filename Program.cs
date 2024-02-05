using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using loveclash_server.UnityPart;
using Newtonsoft.Json;
namespace loveclash_server;
public class Program
{
    static async Task Main()
    {
        var network = new Network();
        await network.Start();
        // FileStream fileStream1 = new FileStream("data_chuilishi",FileMode.Open,FileAccess.ReadWrite);
        // fileStream1.SetLength(0);
        // await NetworkUtility.WriteAsync(fileStream1, Encoding.ASCII.GetBytes("asdfasdf"));
        // fileStream1.Seek(0,SeekOrigin.Begin);
        // var resp = await NetworkUtility.ReadAsync(fileStream1);
        // Console.WriteLine(resp);
    }
}