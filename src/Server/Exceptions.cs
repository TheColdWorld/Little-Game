namespace Server;
class Exception : System.Exception
{
    public Exception(string Message,System.Type type,string ThreadName):base(Message) => Debug.Log(Message,Debug.LogLevel.Error, type, ThreadName);
}
class FatalException : System.Exception
{
    public FatalException(string Message, System.Type type, string ThreadName) : base(Message) => Debug.Log(Message, Debug.LogLevel.Error, type, ThreadName);
}