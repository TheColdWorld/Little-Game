using System.Linq;

namespace Server;
public static class Commands
{
    public static void Help(int page)
    {
        try
        {
            string[] strings = Settings.Language.Helps[page-1];
            Debug.Log(Settings.Language.Help_Head._Format(page, Settings.Language.Helps.Length), Debug.LogLevel.Info, typeof(Commands), System.Threading.Thread.CurrentThread.Name!);
            foreach (string s in strings)
            {
                Debug.Log(s, Debug.LogLevel.Info, typeof(Commands), System.Threading.Thread.CurrentThread.Name!);
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.Log(Settings.Language.Help_OutOfRange._Format(page,Settings.Language.Helps.Length), Debug.LogLevel.Error, typeof(Commands), System.Threading.Thread.CurrentThread.Name!);
        }
    }
}
public class HelpPages
{
    public HelpPages(LanguageJsonFormat format) 
    {
        if (format.Helps.Length < 10)
        {
            _pages= new string[1][] { format.Helps };
        }
        int pages = format.Helps.Length / 10;
        int last = format.Helps.Length % 10;
        System.Collections.Generic.LinkedList<string[]> ret = new();
        for (int i = 0; i < format.Helps.Length - last; i += 10)
        {
            ret.AddLast(format.Helps[i..(i + 9)]);
        }
        ret.AddLast(format.Helps[(10 * pages)..]);
        _pages= ret.ToArray();
    }
    string[][] _pages;
    public string[] this[int page]
    {
        get
        {
            try
            {
                return _pages[page];
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Index out of range", e);
            }
        }
    }
    public int Length => _pages.Length;
    public long LongLength => _pages.LongLength;
}