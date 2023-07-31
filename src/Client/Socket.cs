using System.Linq;

namespace Client;
public class Socket
{
    private static System.Net.Sockets.Socket _Instance;
    public static System.Net.Sockets.Socket Instance
    {
        get
        {
            _Instance ??= new(System.Net.Sockets.AddressFamily.InterNetwork | System.Net.Sockets.AddressFamily.InterNetworkV6, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            return _Instance;
        }
    }

    public class Server
    {
        public Server(System.Net.Sockets.Socket ClientSocket)
        {
            _Socket = ClientSocket;
            _History = new();
        }
        public bool CanUse
        {
            get
            {
                if (_Socket is null) return false;
                if (_History.Count == 3) { if (_History.All(string.IsNullOrEmpty)) return false; }
                return true;
            }
        }
        public string Receive(ulong buffersize = 2048, System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
        {
            string message;
            if (!CanUse) throw new Client.Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            try
            {
                message = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(Recbyte(buffersize, flags))));
            }
            catch (System.FormatException)
            {
                throw new Exception("Package is not current format!", GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
            switch (_History.Count)
            {
                case 0:
                    _History.AddLast(message);
                    return message;
                case 1:
                    _History.AddLast(message);
                    return message;
                case 2:
                    _History.AddLast(message);
                    return message;
                default:
                    _History.AddLast(message);
                    _History.RemoveFirst();
                    return message;
            }

        }
        public async System.Threading.Tasks.Task<string> ReceiveAsync(ulong buffersize = 2048, System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
        {
            string message;
            if (!CanUse) throw new Client.Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            try
            {
                message = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(await RecbyteAsync(buffersize, flags))));
            }
            catch (System.FormatException)
            {
                throw new Exception("Package is not current format!", GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
            switch (_History.Count)
            {
                case 0:
                    _History.AddFirst(message);
                    return message;
                case 1:
                    _History.AddLast(message);
                    return message;
                case 2:
                    _History.AddLast(message);
                    return message;
                default:
                    _History.AddLast(message);
                    _History.RemoveFirst();
                    return message;
            }

        }
        public void Send(string Message)
        {
            if (!CanUse) throw new Client.Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            byte[] sendbyte = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message)));
            try
            {
                _Socket.Send(sendbyte);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                throw new Client.Exception("a " + e.GetType().ToString() + " occored :" + e.Message, GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
        }
        public async void SendAsync(string Message)
        {
            if (!CanUse) throw new Client.Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            byte[] sendbyte = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message)));
            try
            {
                await _Socket.SendAsync(sendbyte);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                throw new Client.Exception("a " + e.GetType().ToString() + " occored :" + e.Message, GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
        }

        byte[] Recbyte(ulong bufsize, System.Net.Sockets.SocketFlags flag)
        {
            byte[] buffer = new byte[bufsize];
            int len = _Socket.Receive(buffer, flag);
            if (len < 1024)
            {
                byte[] ret = new byte[len];
                System.Array.ConstrainedCopy(buffer, 0, ret, 0, len);
                return ret;
            }
            return buffer;
        }
        async System.Threading.Tasks.Task<byte[]> RecbyteAsync(ulong bufsize, System.Net.Sockets.SocketFlags flag)
        {
            byte[] buffer = new byte[bufsize];
            int len = await _Socket.ReceiveAsync(buffer, flag);
            if (len < 1024)
            {
                byte[] ret = new byte[len];
                System.Array.ConstrainedCopy(buffer, 0, ret, 0, len);
                return ret;
            }
            return buffer;
        }


        public System.Net.Sockets.Socket SocketInstance => _Socket;
        System.Net.Sockets.Socket _Socket;
        System.Collections.Generic.LinkedList<string> _History;
    }
}