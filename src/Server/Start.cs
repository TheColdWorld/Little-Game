using System.Linq;
using System.Runtime.InteropServices;

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
                    if (arg == "debug")
                    {
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
            Settings.TaskCandel.Token.Register(() => Debug.Log(Settings.Language.KillThread, Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!));
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
            ExitApplication(0);
        }
        
    }
    public static void ExitApplication(int returncode = 0)
    {
        if (System.Threading.Thread.CurrentThread.Name != "Command Thread") Settings.cmdCandel.Cancel();
        Settings.TaskCandel.Cancel();
        if (Socket.InstanceV6.IsBound) Socket.InstanceV6.Close();
        if (Socket.InstanceV4.IsBound) Socket.InstanceV4.Close(); 
        Debug.Log("Application closed", Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!).GetAwaiter().GetResult();
        Debug.StopWriteFile();
        Debug.LogWrite();
#if DEBUG
        System.Console.ReadKey(true);
#endif
        System.Environment.Exit(0);
    }
    static void ReadSettings()
    {
        string fp = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
        Debug.Log("Start load config from " + fp, Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        SettingJsonFormat jsonf = System.Text.Json.JsonSerializer.Deserialize<SettingJsonFormat>(System.IO.File.ReadAllText(fp))!;
        if (jsonf.LangName is null || jsonf.port is null) throw new FatalException("Invaid Settings", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.port = jsonf.port is null ? 25565 : (int)jsonf.port;
        Settings.LangName = jsonf.LangName;
        Settings.Tmpbuffersize = jsonf.TmpBufferSize is null ? 2048 : (int)jsonf.TmpBufferSize ;
    }
    
}
public static class ServerMain
{
    public static void Run()
    {
        System.Threading.Tasks.Task taskv4, taskv6;
        if (System.Net.Sockets.Socket.OSSupportsIPv4)
        {
            taskv4 = System.Threading.Tasks.Task.Run(() => {
                Socket.InstanceV4.NoDelay = false;
                Socket.InstanceV4.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, Settings.port));
                Socket.InstanceV4.Listen(Settings.port);
                Debug.Log(Settings.Language.OpenListen._Format(Socket.InstanceV4.LocalEndPoint!), Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
            });
        }
        else
        {
            Debug.Log(Settings.Language.OSNotSupportsIPv4, Debug.LogLevel.Warm, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
            taskv4 = System.Threading.Tasks.Task.CompletedTask;
        }
        if (System.Net.Sockets.Socket.OSSupportsIPv6)
        {
            taskv6 = System.Threading.Tasks.Task.Run(() =>
            {
                Socket.InstanceV6.NoDelay = false;
                Socket.InstanceV6.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Any, Settings.port));
                Socket.InstanceV6.Listen(Settings.port);
                Debug.Log(Settings.Language.OpenListen._Format(Socket.InstanceV6.LocalEndPoint!), Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
            });
        }
        else
        {
            Debug.Log(Settings.Language.OSNotSupportsIPv6, Debug.LogLevel.Warm, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
            taskv6 = System.Threading.Tasks.Task.CompletedTask;
        }
        Settings.Clients = new();
        System.Threading.Tasks.Task.WaitAll(taskv4, taskv6);
        Settings.cmdCandel = new();
        Settings.cmdCandel.Token.Register(() => Debug.Log(Settings.Language.KillThread, Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!));
        System.Threading.Tasks.Task.WaitAll(System.Threading.Tasks.Task.Run(async () =>
        {
            for (long i = 1;System.Net.Sockets.Socket.OSSupportsIPv4 ; i++)
            {
                
                System.Net.Sockets.Socket clientsocket =await Socket.InstanceV4.AcceptAsync(Settings.TaskCandel.Token);
                if (clientsocket is null || !clientsocket.Connected || clientsocket.RemoteEndPoint is null) { i--; continue; }
                Debug.Log(Settings.Language.Connect._Format(clientsocket.RemoteEndPoint), Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
                Client client = new(clientsocket, Settings.TaskCandel.Token, "IPv4 -" + i.ToString());
                Settings.Clients.AddLast(client);
                System.Threading.Tasks.Task.Run(() => ClientThread(client), Settings.TaskCandel.Token);
                System.Threading.Thread.Sleep(0);
            }
        }), System.Threading.Tasks.Task.Run(async () =>
        {
            for (long i = 1; System.Net.Sockets.Socket.OSSupportsIPv6; i++)
            {
                System.Net.Sockets.Socket clientsocket = await Socket.InstanceV6.AcceptAsync(Settings.TaskCandel.Token);
                if (clientsocket is null || !clientsocket.Connected || clientsocket.RemoteEndPoint is null) { i--; continue; }
                Debug.Log(Settings.Language.Connect._Format(clientsocket.RemoteEndPoint), Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
                Client client = new(clientsocket, Settings.TaskCandel.Token, "IPv6 -" + i.ToString());
                Settings.Clients.AddLast(client);
                System.Threading.Tasks.Task.Run(() => ClientThread(client ), Settings.TaskCandel.Token);
                System.Threading.Thread.Sleep(0);

            }
        }), System.Threading.Tasks.Task.Run(CommandThread,Settings.cmdCandel.Token)
        );
        
    }
    public async static void ClientThread(Client client)
    {
        System.Threading.Thread.CurrentThread.Name = "Client thread "+ client.ID;
        while (client.SocketInstance.SocketAvailable)
        {
            Debug.Log(await client.Receive(), Debug.LogLevel.Debug, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
            System.Threading.Thread.Sleep(0);
        }
    }
    public static void CommandThread()
    {
        System.Threading.Thread.CurrentThread.Name = "Command Thread";
        while (true)
        {
            string? Command = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(Command)) continue;
            Command = Command.Trim();
            string[] commands=Command.Split(' ',System.StringSplitOptions.RemoveEmptyEntries);
            if (commands.Length == 0) continue;
            switch (commands[0]) 
            { 
                case "exit":
                    Start.ExitApplication(0);
                    break;
                case "stop":
                    Start.ExitApplication(0);
                    break;
                case "help":
                    if(commands.Length == 1 )
                    {
                        Commands.Help(1);
                        continue;
                    }
                    if(!int.TryParse(commands[1], out int page))
                    {
                        Commands.HelpCommand(commands[1]);
                        continue;
                    }
                    Commands.Help(page);
                    break;
                case "log":
                    Debug.LogWrite();
                    Debug.Log(Settings.Language.ForceLogWrite, Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
                    break;
                case "memory":
                    if (commands.Length == 1) Commands.PrintProcessMemoury(string.Empty);
                    else Commands.PrintProcessMemoury(commands[1]);
                    break;
                case "clients":
                    Commands.PrintClients();
                    break;
                default:
                    Debug.Log(Settings.Language.WrongCommand._Format(commands[0]), Debug.LogLevel.Info, typeof(ServerMain), System.Threading.Thread.CurrentThread.Name!);
                    break;
            }
        }
    }
}
