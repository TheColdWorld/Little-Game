namespace Server;
public static class Debug
{
    public static void StopWriteFile()
    {
        WriteTimer.Stop();
        Log("Stopped Log IO", LogLevel.Info, typeof(Debug), System.Threading.Thread.CurrentThread.Name!);
    }
    public static void StartWriteFile()
    {
        LogFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log");
        if (!System.IO.Path.Exists(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs"))) System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs"));
        System.IO.File.Create(LogFilePath).Dispose();
        WriteTimer = new(new System.TimeSpan(0, 0, 30)) { Enabled = true };
        WriteTimer.Elapsed += (obj, e) => LogWrite();
        WriteTimer.Start();
        Log("Started Log IO", LogLevel.Info, typeof(Debug), System.Threading.Thread.CurrentThread.Name!);
    }
    public static void LogWrite()
    {
        if (_WriteFilequeue.Count == 0) return;
        System.IO.FileStream fs = new(LogFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
        fs.Position = fs.Length; string context = string.Empty;
        foreach (string line in _WriteFilequeue) context += line + "\n";
        _WriteFilequeue.Clear();
        fs.Write(System.Text.Encoding.UTF8.GetBytes(context));
        fs.Close();
        fs.Dispose();
        System.GC.Collect();
    }
    /// <summary>
    /// Create a Application Log <br/> It's async
    /// </summary>
    /// <param name="Message">The Message of This log</param>
    /// <param name="level">The log of this level</param>
    /// <param name="AtClass">The class in which the code resides [use typeof()]</param>
    /// <param name="ThreadName">The Current thread name [use System.Threading.Thread.CurrentThread.Name!]</param>
    public static async System.Threading.Tasks.Task Log(string Message, LogLevel level, System.Type AtClass, string ThreadName)
    {
        await System.Threading.Tasks.Task.Run(() =>
        {
            string Context = level switch
            {
                LogLevel.Debug => string.Format("[{0}][{1}/{2}][{3}] : {4}", System.DateTime.Now.ToString(), ThreadName, "Debug", AtClass.FullName, Message.Replace("\n", "\\n")),
                LogLevel.Info => string.Format("[{0}][{1}/{2}][{3}] : {4}", System.DateTime.Now.ToString(), ThreadName, "Info", AtClass.FullName, Message.Replace("\n", "\\n")),
                LogLevel.Warm => string.Format("[{0}][{1}/{2}][{3}] : {4}", System.DateTime.Now.ToString(), ThreadName, "Warm", AtClass.FullName, Message.Replace("\n", "\\n")),
                LogLevel.Error => string.Format("[{0}][{1}/{2}][{3}] : {4}", System.DateTime.Now.ToString(), ThreadName, "Error", AtClass.FullName, Message.Replace("\n", "\\n")),
                LogLevel.FatalError => string.Format("[{0}][{1}/{2}][{3}] : {4}", System.DateTime.Now.ToString(), ThreadName, "Fatal Error", AtClass.FullName, Message.Replace("\n", "\\n")),
                _ => "\n"
            };
            _Log.AddLast(Context);
            _WriteFilequeue.AddLast(Context);
            if (level == LogLevel.Debug) { if (Enable) System.Console.WriteLine(Context); System.Diagnostics.Debug.WriteLine(Context); return; }
            else System.Console.WriteLine(Context);
        });
    }
    public enum LogLevel
    {
        Debug,
        Info,
        Warm,
        Error,
        FatalError,
    }
    private static System.Collections.Generic.LinkedList<string> _Log = new();
    public static string LogFilePath;
    public static System.Collections.Generic.LinkedList<string> _WriteFilequeue = new();
    public static bool Enable = false;
    public static System.Timers.Timer WriteTimer;
}