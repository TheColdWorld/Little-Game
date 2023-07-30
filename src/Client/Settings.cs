namespace Client;

public static class Settings
{
    public static Language lang;
    public static string langName;
    public const string ver ="2.0.0.1";
    public static double loginheight;
    public static double loginwidth;
    public static double mainheight;
    public static double mainwidth;
}

public class SettingJsonFormat
{
    [System.Text.Json.Serialization.JsonPropertyName("language")]
    public string Language { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("loginHeight")]
    public double LoginWindowHeight { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("loginWidth")]
    public double LoginWindowWidth { get; set;}
    [System.Text.Json.Serialization.JsonPropertyName("mainHeight")]
    public double MainWindowHeight { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("mainWidth")]
    public double MainWindowWidth { get; set; }
}