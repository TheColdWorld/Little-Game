using Avalonia.Controls;

namespace Client.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Debug.Log("A new MainWindow instance was Created", Debug.LogLevel.Debug, typeof(MainWindow), System.Threading.Thread.CurrentThread.Name!);
            InitializeComponent();
            Height = Settings.mainheight;
            Width = Settings.mainwidth;
            Title = Settings.lang.mainWindow.Title;
        }
        public static MainWindow Instace 
        { 
            get 
            { 
                _Instace ??= new();
                return _Instace;
            } 
        }
        private static MainWindow _Instace = null ;
    }
}
