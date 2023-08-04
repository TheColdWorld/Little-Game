using System.Linq;

namespace Server;
public static class Settings
{
    public static string LangName;
    public static Language Language;
    public const string ver = "2.0.0.1";
    public static int port;
    public static int Tmpbuffersize;
    public static System.Threading.CancellationTokenSource TaskCandel;
    public static System.Threading.CancellationTokenSource cmdCandel;
    public static Client[] Players =new Client[2];
    public static System.Collections.Generic.LinkedList<Client> Clients;
    public async static void ClearInvaidClientInstance()
    {
        await System.Threading.Tasks.Task.Run(() =>
        {
            System.Collections.Generic.List<Client> clients = Clients.ToList();
            var willremove = from Client c in clients where c.Disposed || c.SocketInstance.Disposed || !c.SocketInstance.SocketInstance.Connected || !c.SocketInstance.SocketAvailable select c;
            for (int i = clients.Count - 1; i >= 0; i--)
            {
                if (willremove.Contains(clients[i]))
                {
                    if (clients[i].Disposed is false) clients[i].Dispose();
                    clients.RemoveAt(i);
                }
            }
            Clients = new(clients);
        });
    }
}
public class SettingJsonFormat
{
    public string? LangName { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("Port")]
    public int? port { get; set; }
    public int? TmpBufferSize { get; set; }
}

