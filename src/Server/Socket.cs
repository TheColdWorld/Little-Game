using System.Linq;

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
    public class Client
    {
        public Client(System.Net.Sockets.Socket ClientSocket)
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
        [System.Obsolete("This method is synchronous, please use Socket.Server.ReceiveAsync", false)]
        public string Receive(ulong buffersize = 2048, System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
        {
            string message;
            if (!CanUse) throw new Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
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
        public async System.Threading.Tasks.Task<string> ReceiveAsync(System.Threading.CancellationToken candeltoken, ulong buffersize = 2048, System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
        {
            string message;
            if (!CanUse) throw new Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            try
            {
                message = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(await RecbyteAsync(buffersize, flags, candeltoken))));
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
        [System.Obsolete("This method is synchronous, please use Socket.Server.SendAsync", false)]
        public void Send(string Message)
        {
            if (!CanUse) throw new Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            byte[] sendbyte = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message)));
            try
            {
                _Socket.Send(sendbyte);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                throw new Exception("a " + e.GetType().ToString() + " occored :" + e.Message, GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
        }
        public async System.Threading.Tasks.Task<int> SendAsync(string Message, System.Threading.CancellationToken candeltoken)
        {
            if (!CanUse) throw new Exception("Invaid Client", GetType(), System.Threading.Thread.CurrentThread.Name!);
            byte[] sendbyte = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message)));
            try
            {
                return await _Socket.SendAsync(sendbyte, candeltoken);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                throw new Exception("a " + e.GetType().ToString() + " occored :" + e.Message, GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
        }
        [System.Obsolete("This method is synchronous, please use Socket.Server.RecbyteAsync", false)]
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
        async System.Threading.Tasks.Task<byte[]> RecbyteAsync(ulong bufsize, System.Net.Sockets.SocketFlags flag, System.Threading.CancellationToken candeltoken)
        {
            byte[] buffer = new byte[bufsize];
            int len = await _Socket.ReceiveAsync(buffer, flag, candeltoken);
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