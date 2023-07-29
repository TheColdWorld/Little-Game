using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;

using System.Linq;

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
        Opened += (s,e) => WindowOpened();
    }
    private void WindowOpened()
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
        button.Click += Login;
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
        _Errmanager = new(this) { MaxItems = 4,Margin=new(1,0)};
    }
    public static Connect Instace
    {
        get
        {
            _Instace ??= new();
            return _Instace;
        }
    }
    private void Login(object? sender, RoutedEventArgs e)
    {
        ref Language lang = ref Settings.lang;
        if (string.IsNullOrWhiteSpace(_IPtextBox.Text))
        {

            ShowNotification("Error",lang.loginWindow.IPWrongError,NotificationType.Error);
            Debug.Log(lang.loginWindow.IPWrongError,Debug.LogLevel.Error,this.GetType(),System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        if (string.IsNullOrEmpty(_PortBox.Text))
        {
            ShowNotification("Error", lang.loginWindow.PortEmptyError, NotificationType.Error);
            Debug.Log(lang.loginWindow.PortEmptyError, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        if (_IPtextBox.Text == "0.0.0.0" || _IPtextBox.Text == "::")
        {
            ShowNotification("Error", lang.loginWindow.IPWrongError, NotificationType.Error);
            Debug.Log(lang.loginWindow.IPWrongError, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        if (!int.TryParse(_PortBox.Text, out int Port))
        {
            ShowNotification("Error", lang.loginWindow.ProtInvaidError, NotificationType.Error);
            Debug.Log(lang.loginWindow.ProtInvaidError, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        if (Port is > 65535 or <= 0)
        {
            ShowNotification("Error", lang.loginWindow.PortOutOfRangeWarm, NotificationType.Error);
            Debug.Log(lang.loginWindow.PortOutOfRangeWarm, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        if (Port <= 1024)
        {
            ShowNotification("Warm", lang.loginWindow.PortLower1024Warm, NotificationType.Warning);
            Debug.Log(lang.loginWindow.PortLower1024Warm, Debug.LogLevel.Warm, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
        }
        if (string.IsNullOrEmpty(_AcctextBox.Text))
        {
            ShowNotification("Error", lang.loginWindow.EmptyAccoutError, NotificationType.Error);
            Debug.Log(lang.loginWindow.EmptyAccoutError, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        if (string.IsNullOrEmpty(_PasswdtextBox.Text))
        {
            ShowNotification("Warm", lang.loginWindow.EmptyPassWordError, NotificationType.Warning);
            Debug.Log(lang.loginWindow.EmptyPassWordError, Debug.LogLevel.Warm, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
        }
        ShowNotification(null, "Start Login", NotificationType.Information);
        Debug.Log("Start Login...",Debug.LogLevel.Info, this.GetType(),System.Threading.Thread.CurrentThread.Name!);
        string CurrentIpaddressstr = _IPtextBox.Text.ToLower() == "localhost" ? "::1" : _IPtextBox.Text;
        System.UriHostNameType IpType= System.Uri.CheckHostName(CurrentIpaddressstr);
        System.Net.IPAddress ip; 
        try
        {
            switch (IpType)
            {
                case System.UriHostNameType.Dns:
                    ip = GetHostAddresses(CurrentIpaddressstr);
                    break;
                case System.UriHostNameType.IPv4:
                    ip = System.Net.IPAddress.Parse(CurrentIpaddressstr);
                    break;
                case System.UriHostNameType.IPv6:
                    ip = System.Net.IPAddress.Parse(CurrentIpaddressstr);
                    break;
                default:
                    ShowNotification("Error", lang.loginWindow.IPWrongError, NotificationType.Error);
                    Debug.Log(lang.loginWindow.IPWrongError, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
                    return;
            }
        }
        catch (System.Net.Sockets.SocketException sockete)
        {
            ShowNotification("Error", sockete.GetType().ToString() + ":\n" + sockete.Message, NotificationType.Error);
            Debug.Log("a " + sockete.GetType().ToString() + " occored :" + sockete.Message, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        catch (Client.Exception)
        {
            ShowNotification("Error", lang.loginWindow.HostDoNotHaveIpError, NotificationType.Error);
            Debug.Log(lang.loginWindow.HostDoNotHaveIpError, Debug.LogLevel.Error, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
            return;
        }
        Debug.Log("Start Login to \'" + ip.ToString() + "\'", Debug.LogLevel.Info, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
    }
    private System.Net.IPAddress GetHostAddresses(string? host)
    {
        System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(host!);
        if (ips.Length == 0) throw new Client.Exception(Settings.lang.loginWindow.IPWrongError, this.GetType(), System.Threading.Thread.CurrentThread.Name!);
        System.Collections.Generic.List<System.Net.IPAddress> ipv6s = ips.Where(e=> e.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6).ToList(); System.Collections.Generic.List<System.Net.IPAddress>? ipv4s = null;
        if (ipv6s.Count == 0) ipv4s = ips.Where(e => e.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
        else return ipv6s[0];
        return ipv4s is null
            ? throw new Client.Exception(Settings.lang.loginWindow.IPWrongError, this.GetType(), System.Threading.Thread.CurrentThread.Name!)
            : ipv4s?.Count == 0
            ? throw new Client.Exception(Settings.lang.loginWindow.IPWrongError, this.GetType(), System.Threading.Thread.CurrentThread.Name!)
            : ipv4s[0];
    }
    private void ShowNotification(string? Title, string? Message, NotificationType notificationType) => _Errmanager?.Show(new Notification(Title, Message, notificationType));
    private WindowNotificationManager _Errmanager;
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
