using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Client;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Debug.Log("starting open Login Window", Debug.LogLevel.Info, typeof(App), System.Threading.Thread.CurrentThread.Name!);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Windows.Connect.Instace;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = Windows.Connect.Instace;
        }
        base.OnFrameworkInitializationCompleted();
    }
}
