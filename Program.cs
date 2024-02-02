using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using loveclash_server.UnityPart;

namespace loveclash_server;

public class Program
{
    
    static async Task Main()
    {
        var network = new Network();
        await network.Start();
    }
}