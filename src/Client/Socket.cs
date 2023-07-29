namespace Client;

public class Socket
{
    public static System.Net.Sockets.Socket Instace
    {
        get
        {
            _Socket ??= new(System.Net.Sockets.AddressFamily.InterNetwork | System.Net.Sockets.AddressFamily.InterNetworkV6,System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            return _Socket;
        }
    }
    private static System.Net.Sockets.Socket? _Socket=null;
}