namespace loveclash_server;

public class NetworkObject
{
    public int networkId = -1;
    public string name;
}

public class NetworkObjectChild : NetworkObject
{
    public int childId = 0;
}