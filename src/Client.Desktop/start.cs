using Avalonia;
using Avalonia.ReactiveUI;
using System.Linq;

namespace Client.Desktop;

internal class Start
{
    [System.STAThread]
    public static void Main(string[] args)
    {
        System.Threading.Thread.CurrentThread.Name = "main";
        System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
        try
        {
            Debug.StartWriteFile();
            FromFileLoadConfig();
            foreach (string arg in args)
            {
                string[] tmp = arg.Split('=', System.StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length > 2) throw new FatalException("Argument format wrong", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                if (tmp.Length == 2)
                {
                    switch (tmp[0].ToLower())
                    {
                        default:
                            throw new FatalException("\'" + tmp[0] + "\' setting not found", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                        case "lang":
                            Settings.langName = Language.GetLangsName().Contains(tmp[1])
                                ? tmp[1]
                                : throw new FatalException("Invaid Language", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                            break;
                        case "mainwindowwidth":
                            Settings.mainwidth = System.Convert.ToDouble(tmp[1]);
                            break;
                        case "mainwindowheight":
                            Settings.mainheight = System.Convert.ToDouble(tmp[1]);
                            break;
                        case "loginwindowheight":
                            Settings.loginheight = System.Convert.ToDouble(tmp[1]);
                            break;
                        case "loginwindowwidth":
                            Settings.loginwidth = System.Convert.ToDouble(tmp[1]);
                            break;
                    }
                    continue;
                }
                if (tmp.Length == 1) throw new FatalException("Argument format wrong", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                if (tmp.Length == 0)
                {
                    Debug.Enable = tmp[0].ToLower() == "debug"
                        ? true
                        : throw new FatalException("\'" + tmp[0] + "\' argument not found", typeof(Start), System.Threading.Thread.CurrentThread.Name!);
                    continue;
                }
            }
            Settings.lang = new(Settings.langName);
            System.GC.Collect();

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(System.Array.Empty<System.String>());
        }
        catch (System.StackOverflowException st)
        {
            Debug.Log("a " + st.GetType().ToString() + " occored:" + st.Message, Debug.LogLevel.FatalError, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        }
        catch (System.Exception e)
        {
            Debug.Log("a "+e.GetType().ToString() + " occored :" + e.Message, Debug.LogLevel.Error, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        }
        finally
        {
            Debug.Log("Application closed", Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
            Debug.WriteTimer.Dispose();
            Debug.LogWrite();
            System.Environment.Exit(0);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace(Avalonia.Logging.LogEventLevel.Information)
        .UseReactiveUI();

    public static void FromFileLoadConfig()
    {
        string fp = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.json");
        Debug.Log("Start load config from" + fp, Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        SettingJsonFormat settings = System.Text.Json.JsonSerializer.Deserialize<SettingJsonFormat>(System.IO.File.ReadAllText(fp, System.Text.Encoding.UTF8))!; Debug.Log("Got data from \'" + fp + "\'", Debug.LogLevel.Info, typeof(Client.Desktop.Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.mainwidth = settings.MainWindowWidth; Debug.Log("Current MainWindowWidth=" + settings.MainWindowWidth.ToString(), Debug.LogLevel.Debug, typeof(Client.Desktop.Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.mainheight = settings.MainWindowHeight; Debug.Log("Current MainWindowHeight=" + settings.MainWindowHeight.ToString(), Debug.LogLevel.Debug, typeof(Client.Desktop.Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.loginheight = settings.LoginWindowHeight; Debug.Log("Current LoginWindowHeight=" + settings.LoginWindowHeight.ToString(), Debug.LogLevel.Debug, typeof(Client.Desktop.Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.loginwidth = settings.LoginWindowWidth; Debug.Log("Current LoginWindowWidth=" + settings.LoginWindowWidth.ToString(), Debug.LogLevel.Debug, typeof(Client.Desktop.Start), System.Threading.Thread.CurrentThread.Name!);
        Settings.langName = settings.Language; Debug.Log("Current language path is\'" + System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "languages", settings.Language)+"\'", Debug.LogLevel.Debug, typeof(Client.Desktop.Start), System.Threading.Thread.CurrentThread.Name!);
    }
}

