using System.Linq;

namespace Server;
public class Client :System.IDisposable
{
    public Client(System.Net.Sockets.Socket ClientSocket,System.Threading.CancellationToken candeltoken,string ID,int tmpbuffersize=2048)
    {
        _SocketCancellationTokenSource= System.Threading.CancellationTokenSource.CreateLinkedTokenSource(candeltoken);
        _socketInstacne = new(ClientSocket, _SocketCancellationTokenSource.Token,tmpbuffersize);
        _ID = ID;
    }
    ~Client()
    {
        _socketInstacne.Dispose();
    }
    public void Dispose()
    {
        _SocketCancellationTokenSource.Cancel();
        _socketInstacne.Dispose();
        _Disposed = true;
        System.GC.SuppressFinalize(this);
    }

    public bool Disposed => _Disposed;
    public Socket SocketInstance => _socketInstacne;
    public string ID => _ID;

    public async void Send(string Message, System.Net.Sockets.SocketFlags flags=System.Net.Sockets.SocketFlags.None)
    {
        try
        {
            if (!_socketInstacne.SocketAvailable) { Dispose(); return; }
           await _socketInstacne.SendAsync(Message,flags);
        }
        catch (System.Net.Sockets.SocketException e)
        {
            switch (e.ErrorCode)
            {
                case 10050:
                    Debug.Log(Settings.Language.SocketErrer.E10050, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    Dispose();
                    break;
                case 10051:
                    Debug.Log(Settings.Language.SocketErrer.E10051, Debug.LogLevel.Error, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    break;
                case 10052:
                    Debug.Log(Settings.Language.SocketErrer.E10052, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    Dispose();
                    break;
                case 10053:
                    Debug.Log(Settings.Language.SocketErrer.E10053, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    break;
                case 10054:
                    Debug.Log(Settings.Language.SocketErrer.E10054, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    Dispose();
                    break;
                case 10060:
                    Debug.Log(Settings.Language.SocketErrer.E10060, Debug.LogLevel.Warm, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    break;
                case 10061:
                    Debug.Log(Settings.Language.SocketErrer.E10061, Debug.LogLevel.Warm, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    break;
                default:
                    throw new Exception("a " + e.GetType().ToString() + " occored :" + e.Message+" return code :" + e.ErrorCode.ToString(), GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
        }
    }

    public async System.Threading.Tasks.ValueTask<string> Receive(System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
    {
        try
        {
            if (!_socketInstacne.SocketAvailable) { Dispose(); return string.Empty; }
            return await _socketInstacne.ReceiveAsync();
        }
        catch (System.Net.Sockets.SocketException e)
        {
            switch (e.ErrorCode)
            {
                case 10050:
                    Debug.Log(Settings.Language.SocketErrer.E10050, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    Dispose();
                    return "";
                case 10051:
                    Debug.Log(Settings.Language.SocketErrer.E10051, Debug.LogLevel.Error, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    return "";
                case 10052:
                    Debug.Log(Settings.Language.SocketErrer.E10052, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    Dispose();
                    return "";
                case 10053:
                    Debug.Log(Settings.Language.SocketErrer.E10053, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    return "";
                case 10054:
                    Debug.Log(Settings.Language.SocketErrer.E10054, Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    Dispose();
                    return "";
                case 10060:
                    Debug.Log(Settings.Language.SocketErrer.E10060, Debug.LogLevel.Warm, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    return "";
                case 10061:
                    Debug.Log(Settings.Language.SocketErrer.E10061, Debug.LogLevel.Warm, GetType(), System.Threading.Thread.CurrentThread.Name!);
                    return "";
                default:
                    throw new Exception("a " + e.GetType().ToString() + " occored :" + e.Message + " return code :" + e.ErrorCode.ToString(), GetType(), System.Threading.Thread.CurrentThread.Name!);
            }
        }
        catch (System.FormatException e)
        {
            if (e.Message == "Package is not current format!")
            {
                Debug.Log(Settings.Language.SocketErrer.WrongPackFormat._Format(_socketInstacne.SocketInstance.RemoteEndPoint!), Debug.LogLevel.Info, GetType(), System.Threading.Thread.CurrentThread.Name!);
                return string.Empty;
            }
            else throw new System.Exception(null, e);
        }
    }

    System.Threading.CancellationTokenSource _SocketCancellationTokenSource;
    bool _Disposed = false;
    private Socket _socketInstacne;
    private readonly string _ID;

    public class Socket : System.IDisposable
    {
        public Socket(System.Net.Sockets.Socket ClientSocket, System.Threading.CancellationToken cancellation,int buffersize=2048)
        {
            _Socket = ClientSocket;
            _Socket.SendBufferSize = buffersize * 5;
            _Socket.ReceiveBufferSize = buffersize * 5;
             _cancellationToken = cancellation;
            _SocketHistory = new();
            _cancellationToken.Register(_Socket.Close);
        }
        ~Socket()
        {
            _Socket.Close();
            _Socket.Dispose();
        }
        public void Dispose()
        {
            _Socket.Close();
            _Socket.Dispose();
            _Disposed = true;
            System.GC.SuppressFinalize(this);
        }

        public bool Disposed => _Disposed;
        public bool SocketAvailable
        {
            get
            {
                if(_Disposed) return false;
                if (_Socket is null) return false;
                if (_SocketAvailable == false) return false;
                if (_SocketHistory.Count == 3) { if (_SocketHistory.All(string.IsNullOrEmpty)) return false; }
                return true;
            }
        }

        async System.Threading.Tasks.ValueTask<byte[]> Recbyte(System.Net.Sockets.SocketFlags flag)
        {
            byte[] buffer = new byte[_Socket.ReceiveBufferSize / 5];
            int len = await _Socket.ReceiveAsync(buffer, flag,_cancellationToken);
            if (len < 1024)
            {
                byte[] ret = new byte[len];
                System.Array.ConstrainedCopy(buffer, 0, ret, 0, len);
                return ret;
            }
            return buffer;
        }
        async System.Threading.Tasks.Task AddHistory(string msg)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                switch (_SocketHistory.Count)
                {
                    case 0:
                        _SocketHistory.AddLast(msg);
                        break;
                    case 1:
                        _SocketHistory.AddLast(msg);
                        break;
                    case 2:
                        _SocketHistory.AddLast(msg);
                        break;
                    default:
                        _SocketHistory.AddLast(msg);
                        _SocketHistory.RemoveFirst();
                        break;
                }

            });
        }
        public string Receive(System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
        {
            string message;
            if (!SocketAvailable || _Disposed) throw new System.Exception("Socket is not available!");
            try
            {
                message = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(Recbyte(flags).GetAwaiter().GetResult())));
            }
            catch (System.FormatException)
            {
                throw new System.Exception("Package is not current format!");
            }
            AddHistory(message);
            return message;
        }
        public async System.Threading.Tasks.ValueTask<string> ReceiveAsync(System.Net.Sockets.SocketFlags flags = System.Net.Sockets.SocketFlags.None)
        {
            string message;
            if (!SocketAvailable || _Disposed) throw new System.Exception("Socket is not available!");
            try
            {
                message = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Text.Encoding.UTF8.GetString( await Recbyte(flags))));
            }
            catch (System.FormatException)
            {
                AddHistory(string.Empty);
                throw new System.FormatException("Package is not current format!");
            }
            AddHistory(message);
            return message;
        }
        public void Send(string Message, System.Net.Sockets.SocketFlags flags)
        {
            if (!SocketAvailable || _Disposed) throw new System.Exception("Socket is not available!");
            byte[] sendbyte = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message)));
            _Socket.SendAsync(sendbyte,flags,_cancellationToken).AsTask().GetAwaiter().GetResult();
        }
        public async System.Threading.Tasks.Task SendAsync(string Message, System.Net.Sockets.SocketFlags flags)
        {
            if (!SocketAvailable || _Disposed) throw new System.Exception("Socket is not available!");
            byte[] sendbyte = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message)));
            await _Socket.SendAsync(sendbyte,flags, _cancellationToken) ;
        }

        public System.Net.Sockets.Socket SocketInstance => _Socket;
        System.Net.Sockets.Socket _Socket;
        System.Collections.Generic.LinkedList<string> _SocketHistory;
        System.Threading.CancellationToken _cancellationToken;
        bool _Disposed = false;
        bool _SocketAvailable = true;
    }
}