using System.Linq;

namespace Server;
class Start
{
    public static void Main(string[] args)
    {
        System.Threading.Thread.CurrentThread.Name = "main";
        System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
        try
        {
            Debug.StartWriteFile();
            ReadSettings();
            foreach (string arg in args)
            {
                string[] tmp = arg.Split('=', System.StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 1)
                {
                    if(arg == "debug") {
                        Debug.Enable = tmp[0].ToLower() == "debug"
                        ? true
                        : throw new FatalException("\'" + tmp[0] + "\' argument not found", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                    }
                    continue;
                }
                if (tmp.Length == 0) throw new FatalException("Argument format wrong", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                if (tmp.Length == 2)
                {
                    switch (tmp[0].ToLower())
                    {
                        case "lang":
                            Settings.LangName = Language.GetLangsName().Contains(tmp[1])
                                ? tmp[1]
                                : throw new FatalException("Invaid Language", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                            break;
                        case "port":
                            {
                                if (!int.TryParse(tmp[1], out int port)) throw new FatalException("Invaid Port number", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                                Settings.port = port;
                            }
                            break;
                        default:
                            throw new FatalException("\'" + tmp[0] + "\' setting not found", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                    }
                    continue;
                }
            }
            Settings.Language = new(Settings.LangName);
            System.GC.Collect();
            Settings.TaskCandel = new();
            Settings.TaskCandel.Token.Register(() => Debug.Log("Thread " + System.Environment.CurrentManagedThreadId.ToString() + " killed", Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!));
            Debug.Log(Settings.Language.StartServer, Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name);
            ServerMain.Run();
        }
        //catch (Server.Exception se)
        //{
        //    Debug.Log("a " + se.GetType().ToString() + " occored :" + se.Message, Debug.LogLevel.Error, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        //}
        //catch (Server.FatalException sfe)
        //{
        //    Debug.Log("a " + sfe.GetType().ToString() + " occored :" + sfe.Message, Debug.LogLevel.FatalError, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        //}
        //catch (System.Exception sye)
        //{
        //    Debug.Log("a " + sye.GetType().ToString() + " occored :" + sye.Message, Debug.LogLevel.Error, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        //}
        finally
        {
            Settings.TaskCandel.Cancel();
            Socket.InstanceV4.Close(); Socket.InstanceV6.Close();
            Debug.Log("Application closed", Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
            Debug.StopWriteFile();
            Debug.WriteTimer.Dispose();
            Debug.LogWrite();
#if DEBUG
            System.Console.ReadKey(true);
#endif
            System.Environment.Exit(0);
        }
        
    }

    static void ReadSettings()
    {
        string fp = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
        Debug.Log("Start load config from " + fp, Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        SettingJsonFormat jsonf = System.Text.Json.JsonSerializer.Deserialize<SettingJsonFormat>(System.IO.File.ReadAllText(fp))!;
        if (jsonf.LangName is null || jsonf.port is null) throw new FatalException("Invaid Settings", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.port = (int)jsonf.port;
        Settings.LangName = jsonf.LangName;
    }
    
}
public static class ServerMain
{
    public static void Run()
    {
        Socket.InstanceV4.NoDelay = false; Socket.InstanceV6.NoDelay = false;
        Socket.InstanceV4.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, Settings.port));
        Socket.InstanceV4.Listen(Settings.port);
        Socket.InstanceV6.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Any, Settings.port));
        Socket.InstanceV6.Listen(Settings.port);
        System.Threading.Tasks.Task.WaitAll(System.Threading.Tasks.Task.Run(() =>
        {
            for (long i = 1; ; i++)
            {
                System.Net.Sockets.Socket clientsocket = Socket.InstanceV4.Accept();
                if (clientsocket is null || !clientsocket.Connected || clientsocket.RemoteEndPoint is null) { i--; continue; }
                Debug.Log("new connect ip: " + clientsocket.RemoteEndPoint.ToString() + " was created", Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
                System.Threading.Tasks.Task.Run(() => ClientThread(clientsocket,"V4 -"+ i.ToString()), Settings.TaskCandel.Token);
            }
        }), System.Threading.Tasks.Task.Run(() =>
        {
            for (long i = 1; ; i++)
            {
                System.Net.Sockets.Socket clientsocket = Socket.InstanceV6.Accept();
                if (clientsocket is null || !clientsocket.Connected || clientsocket.RemoteEndPoint is null) { i--; continue; }
                Debug.Log("new connect ip: \'" + clientsocket.RemoteEndPoint.ToString() + "\' was created", Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
                System.Threading.Tasks.Task.Run(() => ClientThread(clientsocket,"V6 -" +i.ToString()), Settings.TaskCandel.Token);
            }
        }));
        
    }
    public static void ClientThread(System.Net.Sockets.Socket clientsocket,string ThreadID)
    {
        System.Threading.Thread.CurrentThread.Name = "Client thread "+ ThreadID;
        while (clientsocket.Poll(100,System.Net.Sockets.SelectMode.SelectWrite | System.Net.Sockets.SelectMode.SelectRead))
        {
            string message = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(clientsocket.Receive(System.Net.Sockets.SocketFlags.None))));
            Debug.Log(message, Debug.LogLevel.Debug, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
        }
    }
    public static byte[] Receive(this System.Net.Sockets.Socket socket,System.Net.Sockets.SocketFlags flag)
    {
        if(!socket.Poll(100, System.Net.Sockets.SelectMode.SelectWrite | System.Net.Sockets.SelectMode.SelectRead)) return System.Array.Empty<byte>();
        byte[] buffer=new byte[2048];
        int len= socket.Receive(buffer,flag);
        if (len <= 0)
        {
            return System.Array.Empty<byte>();
        }
        if (len < 2048)
        {
            byte[] ret = new byte[len];
            try
            {
                System.Array.ConstrainedCopy(buffer, 0, ret, 0, len);
            }
            catch (System.Exception)
            {
                return System.Array.Empty<byte>();
            }
            return ret;
        }
        else
        {
            while (true)
            {
                byte[] tmp = new byte[2048];
                len = socket.Receive(buffer,2048, flag);
                if (len < 2048)
                {
                    byte[] tmp2 = new byte[len];
                    System.Array.ConstrainedCopy(tmp, 0, tmp2, 0, len);
                    buffer = buffer.Concat(tmp2).ToArray();
                    break;
                }
                else
                {
                    buffer = buffer.Concat(tmp).ToArray();
                    continue;
                }
            }
            return buffer;
        }
    }
}
