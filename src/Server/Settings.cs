namespace Server;
public static class Settings
{
    public static string LangName;
    public static Language Language;
    public const string ver = "2.0.0.1";
    public static int port;
    public static System.Threading.CancellationTokenSource TaskCandel;
    public static System.Threading.CancellationTokenSource cmdCandel;
}
public class SettingJsonFormat
{
    public string? LangName { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("Port")]
    public int? port { get; set; }
}