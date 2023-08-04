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
        __SocketErrer = new(jsonf);
        _OSNotSupportsIPv4 = jsonf.OSNotSupportsIPv4;
        _OSNotSupportsIPv6= jsonf.OSNotSupportsIPv6;
        _Connect= jsonf.Connect;
        _OpenListen = jsonf.OpenListen;
        _ForceLogWrite = jsonf.ForceLogWrite;
        _ProcessMemory=jsonf.ProcessMemory;
    }
    public class _SocketErrer
    {
        public _SocketErrer(LanguageJsonFormat format)
        {
            _E10050 = format.socketErrer.E10050;
            _E10051 = format.socketErrer.E10051;
            _E10052 = format.socketErrer.E10052;
            _E10053 = format.socketErrer.E10053;
            _E10054 = format.socketErrer.E10054;
            _E10060 = format.socketErrer.E10060;
            _E10061 = format.socketErrer.E10061;
            _WrongPackFormat=format.socketErrer.WrongPackFormat;
        }
        public string WrongPackFormat => _WrongPackFormat;
        public string E10050 => _E10050;
        public string E10051 => _E10051;
        public string E10052 => _E10052;
        public string E10053 => _E10053;
        public string E10054 => _E10054;
        public string E10060 => _E10060;
        public string E10061 => _E10061;

        private readonly string _WrongPackFormat;
        private readonly string _E10050;
        private readonly string _E10051;
        private readonly string _E10052;
        private readonly string _E10053;
        private readonly string _E10054;
        private readonly string _E10060;
        private readonly string _E10061;
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
    private readonly _SocketErrer __SocketErrer;
    private readonly string _OSNotSupportsIPv4;
    private readonly string _OSNotSupportsIPv6;
    private readonly string _Connect;
    private readonly string _OpenListen;
    private readonly string _ForceLogWrite;
    private readonly string _ProcessMemory;
    public string ProcessMemory => _ProcessMemory;
    public string ForceLogWrite => _ForceLogWrite;
    public string OpenListen => _OpenListen;
    public string Connect => _Connect;
    public string OSNotSupportsIPv4 => _OSNotSupportsIPv4;
    public string OSNotSupportsIPv6 => _OSNotSupportsIPv6;
    public _SocketErrer SocketErrer => __SocketErrer;
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
    public string OSNotSupportsIPv4 { get; set; }
    public string OSNotSupportsIPv6 { get; set; }
    public string KillThread { get; set; }  
    public string WrongCommand { get; set; }
    public string[] Helps { get; set; }
    public string Help_OutOfRange { get;set; }
    public string Help_Head { get; set; }
    public string Help_ArgWrong { get; set; }
    public string Connect { get;set; }
    public string OpenListen { get; set; }
    public string ForceLogWrite { get; set; }
    public string ProcessMemory { get;set; }
    [System.Text.Json.Serialization.JsonPropertyName("SocketErrer")]
    public SocketErrer socketErrer { get; set; }
    public class SocketErrer
    {
        public string E10050 { get; set; }
        public string E10051 { get; set; }
        public string E10052 { get; set; }
        public string E10053 { get; set; }
        public string E10054 { get; set; }
        public string E10060 { get; set; }
        public string E10061 { get;set; }
        public string WrongPackFormat { get; set; }
    }
}
public static class LangReplace
{
    public static string _Format(this string i, Language lang) => i.Replace("{Ver}", Settings.ver).Replace("{LangVer}", lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", lang.Author);
    public static string _Format(this string i,int current,int max)=> i.Replace("{current}",current.ToString()).Replace("{Max}",max.ToString());
    public static string _Format(this string i, string cmd) => i.Replace("{cmd}", cmd);
    public static string _Format(this string i, string Arg,string from) => i.Replace("{Arg}", Arg).Replace("{From}",from);
    public static string _Format(this string i, System.Net.EndPoint ipe) => i.Replace("{IP}", ipe.ToString());
    public static string _Format(this string i, System.Diagnostics.Process proc) => i.Replace("{mem}", (proc.WorkingSet64 /1024).ToString());
}