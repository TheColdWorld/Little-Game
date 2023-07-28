namespace Client;
public class Exception : System.Exception
{
    public Exception(string Message, System.Type AtClass, string ThreadName) : base(Message) => Debug.Log("Exception occured: " + Message, Debug.LogLevel.Error, AtClass, ThreadName);
}
public class FatalException : System.Exception
{
    public FatalException(string Message, System.Type AtClass, string ThreadName) : base(Message)
    {
        Debug.Log("Fatal Exception occured: " + Message,Debug.LogLevel.Error, AtClass, ThreadName);Debug.LogWrite();
    }
}