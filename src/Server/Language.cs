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
        _KillThread = jsonf.KillThread;
        _WrongCommand= jsonf.WrongCommand;
        _Helps = new(jsonf);
        _Help_OutOfRange = jsonf.Help_OutOfRange;
        _Help_Head=jsonf.Help_Head;
        _Help_ArgWrong= jsonf.Help_ArgWrong;
    }
    public static System.Collections.Generic.IEnumerable<string> GetLangsName()
    {
        foreach (var file in new System.IO.DirectoryInfo(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "languages")).GetFiles()) yield return file.Name;
    }
    private readonly string _Name;
    private readonly string _Author;
    private readonly string _StartServer;
    private readonly string _Version;
    private readonly string _KillThread;
    private readonly string _WrongCommand;
    private readonly HelpPages _Helps;
    private readonly string _Help_OutOfRange;
    private readonly string _Help_Head;
    private readonly string _Help_ArgWrong;
    public string Help_ArgWrong => _Help_ArgWrong;
    public string Help_Head => _Help_Head;
    public string Help_OutOfRange => _Help_OutOfRange;
    public HelpPages Helps => _Helps;
    public string WrongCommand => _WrongCommand;
    public string KillThread => _KillThread;
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
    public string KillThread { get; set; }  
    public string WrongCommand { get; set; }
    public string[] Helps { get; set; }
    public string Help_OutOfRange { get;set; }
    public string Help_Head { get; set; }   
    public string Help_ArgWrong { get; set; }   
}
public static class LangReplace
{
    public static string _Format(this string i, Language lang) => i.Replace("{Ver}", Settings.ver).Replace("{LangVer}", lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", lang.Author);
    public static string _Format(this string i,int current,int max)=> i.Replace("{current}",current.ToString()).Replace("{Max}",max.ToString());
    public static string _Format(this string i, string cmd) => i.Replace("{cmd}", cmd);
    public static string _Format(this string i, string Arg,string from) => i.Replace("{Arg}", Arg).Replace("{From}",from);
}