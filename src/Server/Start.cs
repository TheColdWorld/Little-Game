namespace Server;
class Start
{
    public static void Main(string[] args)
    {
        System.Threading.Thread.CurrentThread.Name = "main";
        System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out));
        Debug.StartWriteFile();
        try
        {

        }
        catch (Server.Exception se)
        {
            Debug.Log("a " + se.GetType().ToString() + " occored :" + se.Message, Debug.LogLevel.Error, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        }
        catch (Server.FatalException sfe)
        {
            Debug.Log("a " + sfe.GetType().ToString() + " occored :" + sfe.Message, Debug.LogLevel.FatalError, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        }
        catch (System.Exception sye)
        {
            Debug.Log("a " + sye.GetType().ToString() + " occored :" + sye.Message, Debug.LogLevel.Error, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
        }
        finally
        {
            Debug.Log("Application closed", Debug.LogLevel.Info, typeof(Start), System.Threading.Thread.CurrentThread.Name!);
            Debug.WriteTimer.Dispose();
            Debug.LogWrite();
            System.Environment.Exit(0);
        }
    }
}