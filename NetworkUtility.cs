using System.Net.Sockets;
using System.Text;
using loveclash_server.UnityPart;
using Newtonsoft.Json;

namespace loveclash_server;

public static class NetworkUtility
{
    public static async Task<string> ReadAsync(Stream stream)
    {
        //先预读一下长度(前4个byte, 即一个字节)
        var buffer = new byte[4];
        var respSize = await stream.ReadAsync(buffer, 0, buffer.Length);
        if (respSize < 4)
        {
            Console.WriteLine("respSize是: "+respSize);
            return string.Empty;
        }
        //cast buffer to int
        int messageSize = BitConverter.ToInt32(buffer, 0);
        Console.WriteLine("读入的字节数为: "+messageSize);
        buffer = new byte[messageSize];
        try
        {
            await stream.ReadAsync(buffer,0,messageSize);
            if(messageSize==0)return string.Empty;
        }
        catch (Exception e)
        {
            Logger.Log("Receive 中断");
            throw;
        }
        var formatted = new byte[messageSize];
        Array.Copy(buffer,formatted,messageSize);
        Console.WriteLine("读入的数据: "+Encoding.ASCII.GetString(formatted));
        return Encoding.ASCII.GetString(formatted);
    }
    public static async Task<string> ReadAsync(TcpClient client)
    {
        return await ReadAsync(client.GetStream());
    }
    public static async Task WriteAsync(TcpClient client,string s)
    {
        await WriteAsync(client, Encoding.ASCII.GetBytes(s));
    }
    public static async Task WriteAsync(Stream stream,byte[] buffer)
    {
        try
        {
            //把一个代表数据长度的int写在前面
            var bufferHead = BitConverter.GetBytes(buffer.Length);
            byte[] bufferToBeSend = new byte[bufferHead.Length+buffer.Length];
            Array.Copy(bufferHead,0,bufferToBeSend,0,bufferHead.Length);
            Array.Copy(buffer,0,bufferToBeSend,bufferHead.Length,buffer.Length);
            await stream.WriteAsync(bufferToBeSend,0,bufferToBeSend.Length);
            await stream.FlushAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("写错误");
            throw;
        }
    }
    public static async Task WriteAsync(TcpClient client, byte[] buffer)
    {
        await WriteAsync(client.GetStream(), buffer);
    }
    public static async Task BroadCast(byte[] bytes,params TcpClient[] clients)
    {
        try
        {
            List<Task> tasks = new List<Task>();
            foreach (var cl in clients)
            {
                tasks.Add(WriteAsync(cl,bytes));
            }
            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public static async Task BroadCast(string s,params TcpClient[] clients)
    {
        await BroadCast(Encoding.ASCII.GetBytes(s),clients);
    }
    public static async Task BroadCast(Operation operation,params TcpClient[] clients)
    {
        await BroadCast(JsonConvert.SerializeObject(operation,Network.jsonSetting),clients);
    }
}