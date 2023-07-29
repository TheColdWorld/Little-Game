namespace Server;
class Socket
{
    private static System.Net.Sockets.Socket _Instance;
    public static System.Net.Sockets.Socket Instance => _Instance;
}