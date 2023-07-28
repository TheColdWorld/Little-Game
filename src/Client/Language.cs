using System.Diagnostics;

namespace Client;
public class Language
{
    public static System.Collections.Generic.IEnumerable<string> GetLangsName()
    {
        foreach(var file in new System.IO.DirectoryInfo(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "languages")).GetFiles()) yield return file.Name;
    }
    public Language(string Langname)
    {
        Debug.Log("Start get language from \'" + Langname + "\'", Debug.LogLevel.Info, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
        LanguageJsonFormat format = System.Text.Json.JsonSerializer.Deserialize<LanguageJsonFormat>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "languages", Langname)))!;
        _Name=format.Name; Debug.Log("Current Language Name=" + format.Name, Debug.LogLevel.Debug, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
        _Version = format.Version; Debug.Log("Current Language Version=" + format.Version, Debug.LogLevel.Debug, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
        _Auther = format.Author; Debug.Log("Current Language Author=" + format.Author, Debug.LogLevel.Debug, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
        _MainWindow = new(format.mainwindow); Debug.Log("Current Language mainwindow =>" + format.mainwindow.ToString(), Debug.LogLevel.Debug, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
        _LoginWindow = new(format.loginwindow); Debug.Log("Current Language loginwindow =>" + format.loginwindow.ToString(), Debug.LogLevel.Debug, typeof(Language), System.Threading.Thread.CurrentThread.Name!);
    }
    private string _Name;
    private string _Auther;
    private string _Version;
    private MainWindow _MainWindow;
    private LoginWindow _LoginWindow;
    public MainWindow mainWindow => _MainWindow;
    public LoginWindow loginWindow => _LoginWindow;
    public string Name => _Name;
    public string Author => _Auther;
    public string Version => _Version;
    public class MainWindow
    {
        public MainWindow(LanguageJsonFormat.MainWindow mainWindow) => _Title= mainWindow.Title;
        public string Title => _Title.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        private string _Title;
        public override string ToString() => "_title=" + _Title;
    }
    public class LoginWindow
    {
        public LoginWindow(LanguageJsonFormat.LoginWindow loginWindow)
        {
            _Title = loginWindow.Title;
            _Accout=loginWindow.Accout;
            _IP=loginWindow.IP;
            _PassWord=loginWindow.PassWord;
            _Button=loginWindow.Button;
            _Port = loginWindow.Port;
        }
        public string Title => _Title.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        private string _Title;
        private string _IP;
        private string _Accout;
        private string _PassWord;
        private string _Button;
        private string _Port;
        public string Port => _Port.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        public string Button => _Button.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        public string IP => _IP.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        public string Accout => _Accout.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        public string PassWord => _PassWord.Replace("{Ver}", Settings.ver).Replace("{LangVer}", Settings.lang.Version).Replace("{Auther}", "TheColdWorld").Replace("{LangAuther}", Settings.lang.Author);
        public override string ToString() => "_title=" + _Title;
    }
}
public class LanguageJsonFormat
{
    public string Name { get; set; }
    public string Author { get; set; }
    public string Version { get;set; }
    [System.Text.Json.Serialization.JsonPropertyName("MainWindow")]
    public MainWindow mainwindow { get; set; }
    public class MainWindow
    {
        // {Ver} -> Settings.Version
        // {LangVer} -> Language.Version
        // {Auther} -> "TheColdWorld"
        // {LangAuther} -> Language.Author
        public string Title { get; set; }
        public override string ToString() => "_title=" + Title;
    }
    [System.Text.Json.Serialization.JsonPropertyName("LoginWindow")]
    public LoginWindow loginwindow { get; set; }
    public class LoginWindow
    {
        // {Ver} -> Settings.Version
        // {LangVer} -> Language.Version
        // {Auther} -> "TheColdWorld"
        // {LangAuther} -> Language.Author
        public string Title { get; set; }
        public string IP { get; set; }
        public string Accout { get;set; }
        public string PassWord { get; set; }    
        public string Button { get;set; }
        public string Port { get; set; }    
        public override string ToString() => "_title=" + Title;
    }
}