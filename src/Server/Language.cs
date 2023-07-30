namespace Server;
public class Language
{
    public Language(string name)
    {
        string fp =System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Languages",name);
        Debug.Log("Start get language from \'" + fp + "\'", Debug.LogLevel.Info, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
        LanguageJsonFormat jsonf = System.Text.Json.JsonSerializer.Deserialize<LanguageJsonFormat>(System.IO.File.ReadAllText(fp))!;
        _Author = jsonf.Author;
        _Name = jsonf.Name;
        _StartServer= jsonf.StartServer;
        _Version = jsonf.Version;
    }
    public static System.Collections.Generic.IEnumerable<string> GetLangsName()
    {
        foreach (var file in new System.IO.DirectoryInfo(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "languages")).GetFiles()) yield return file.Name;
    }
    private readonly string _Name;
    private readonly string _Author;
    private readonly string _StartServer;
    private readonly string _Version;
    public string Name => _Name;
    public string Author => _Author;
    public string StartServer => _StartServer._Format(this);
    public string Version => _Version;
}
public class LanguageJsonFormat
{
    public string Name { get;set; }
    public string Version { get; set; }
    public string Author { get; set; }
    public string StartServer { get; set; }
}
public static class LangReplace
{
    public static string _Format(this string i, Language lang) => i.Replace("{Ver}", Settings.ver).Replace("{LangVer}", lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", lang.Author);
}