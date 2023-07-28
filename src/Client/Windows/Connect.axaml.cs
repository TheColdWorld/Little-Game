using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Client.Windows;
public partial class Connect : Window
{
    public Connect()
    {
        Debug.Log("A new Connect instance was Created", Debug.LogLevel.Debug, typeof(Connect), System.Threading.Thread.CurrentThread.Name!);
        InitializeComponent();
        Height = Settings.loginheight;
        Width = Settings.loginwidth;
        Title = Settings.lang.mainWindow.Title;
        WindowOpened();
    }
    public void WindowOpened()
    {
        ref Language Lang = ref Settings.lang;
        Ip = new() { MaxWidth = Width, MaxHeight = Height / 5 };
        Acc = new() { MaxWidth = Width, MaxHeight = Height / 5 };
        Passwd = new() { MaxWidth = Width, MaxHeight = Height /5 };
        Port = new() { MaxWidth = Width, MaxHeight = Height / 5 };
        TextBlock Ipblock = new() { TextAlignment=Avalonia.Media.TextAlignment.Left,HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left ,Text= Lang.loginWindow.IP};
        TextBlock Accblock = new() { TextAlignment = Avalonia.Media.TextAlignment.Left, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Text = Lang.loginWindow.Accout };
        TextBlock Passwdblock = new() { TextAlignment = Avalonia.Media.TextAlignment.Left, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Text = Lang.loginWindow.PassWord };
        TextBlock Portblock = new() { TextAlignment = Avalonia.Media.TextAlignment.Left, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Text = Lang.loginWindow.Port };
        _IPtextBox =new() { TextAlignment = Avalonia.Media.TextAlignment.Left };
        _AcctextBox = new() { TextAlignment = Avalonia.Media.TextAlignment.Left };
        _PasswdtextBox = new() { TextAlignment = Avalonia.Media.TextAlignment.Left };
        _PortBox = new() { TextAlignment = Avalonia.Media.TextAlignment .Left };
        Ip.Children.Add(Ipblock);
        Ip.Children.Add(_IPtextBox);
        Acc.Children.Add(Accblock);
        Acc.Children.Add(_AcctextBox);
        Passwd.Children.Add(Passwdblock);
        Passwd.Children.Add(_PasswdtextBox);
        Port.Children.Add(Portblock);
        Port.Children.Add(_PortBox);
        MainStackPanel.Children.Add(Ip);
        MainStackPanel.Children.Add(Port);
        MainStackPanel.Children.Add(Acc);
        MainStackPanel.Children.Add(Passwd);
        button = new() { MaxWidth = Width, MaxHeight = Height / 4, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,Content=Lang.loginWindow.Button,Name="login"};
        MainStackPanel.Children.Add(button);
        SizeChanged += (o, e) =>
        {
            MainStackPanel.Height = Height;
            MainStackPanel.Width = Width;
            Ip.HorizontalAlignment=Avalonia.Layout.HorizontalAlignment .Left;
            Acc.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            Port.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            Passwd.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            button.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
        };
    }
    public static Connect Instace
    {
        get
        {
            _Instace ??= new();
            return _Instace;
        }
    }
    private void Login(object sender, RoutedEventArgs e)
    {
        
    }
    private static Connect _Instace = null;
    private TextBox _IPtextBox;
    private TextBox _AcctextBox;
    private TextBox _PortBox;
    private TextBox _PasswdtextBox;
    private StackPanel Ip;
    private StackPanel Acc;
    private StackPanel Port;
    private StackPanel Passwd;
    private Button button;
}
