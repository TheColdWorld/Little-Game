namespace Server;
public class Socket
{
    private static System.Net.Sockets.Socket _InstanceV4;
    public static System.Net.Sockets.Socket InstanceV4
    {
        get
        {
            _InstanceV4 ??= new(System.Net.Sockets.AddressFamily.InterNetwork , System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            return _InstanceV4;
        }
    }
    private static System.Net.Sockets.Socket _InstanceV6;
    public static System.Net.Sockets.Socket InstanceV6
    {
        get
        {
            _InstanceV6 ??= new(System.Net.Sockets.AddressFamily.InterNetworkV6, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            return _InstanceV6;
        }
    }
}