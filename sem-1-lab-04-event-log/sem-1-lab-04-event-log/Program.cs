namespace Log;

public class LogEntry
{
    public DateTime Timestamp;
    public string Level;
    public string Category;
    public string Message;
}
public class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines("event_server.log");
        
        LogEntry[] entries = ParseLog(lines);
        string status = GetServerStatus(entries);
        Console.WriteLine($"Статус сервера: {status}");

        
    }

    public static LogEntry[] ParseLog(string[] lines)
    {
        LogEntry[] result = new LogEntry[lines.Length];
        for (int i=0; i<lines.Length; i++)
        {
            string line = lines[i];
            LogEntry entry = new LogEntry();
            entry.Timestamp = DateTime.Parse(line.Substring(0,23));

            int firstIpenBracket = line.IndexOf('[');
            int firstClosedBracket = line.IndexOf(']', firstIpenBracket);
            int secondOpenBracket = line.IndexOf('[', firstClosedBracket);
            int secondClosedBracked = line.IndexOf(']', secondOpenBracket);

            entry.Level = line.Substring(firstIpenBracket + 1, firstClosedBracket - firstIpenBracket - 1);
            entry.Category = line.Substring(secondOpenBracket + 1, secondClosedBracked - secondOpenBracket - 1);
            entry.Message = line.Substring(secondClosedBracked + 2);

            result[i] = entry;
        }

        return result;
    }

    public static LogEntry[] FilterByDate(LogEntry[] entries, DateTime date)
    {
        LogEntry[] temproraryResult = new LogEntry[entries.Length];
        int resCount = 0;

        for(int i =0; i < entries.Length; i++)
        {
            if (entries[i].Timestamp.Date == date.Date)
            {
                temproraryResult[resCount] = entries[i];
                resCount++;
            }
        }
        LogEntry[] result = new LogEntry[resCount];
        Array.Copy(temproraryResult, result, resCount);
        return result;
    }

    public static LogEntry[] FilterByLevel(LogEntry[] entries, string level)
    {
        LogEntry[] temproraryResult = new LogEntry[entries.Length];
        int resCount = 0;

        for (int i=0; i < entries.Length; i++)
        {
            if (entries[i].Level.ToLower() == level.ToLower())
            {
                temproraryResult[resCount] = entries[i];
                resCount++;
            }
        }
        LogEntry[] res = new LogEntry[resCount];
        Array.Copy(temproraryResult, res, resCount);
        return res;
    }

    public static LogEntry[] FilterByCategory(LogEntry[] entries, string category)
    {
        LogEntry[] temproraryResult = new LogEntry[entries.Length];
        int resCount = 0;

        for (int i=0; i < entries.Length; i++)
        {
            if (entries[i].Category.ToLower() == category.ToLower())
            {
                temproraryResult[resCount] = entries[i];
                resCount++;
            }
        }
        LogEntry[] res = new LogEntry[resCount];
        Array.Copy(temproraryResult, res, resCount);
        return res;
    }

    public static LogEntry[] Search(LogEntry[] entries, string text)
    {
        LogEntry[] temproraryResult = new LogEntry[entries.Length];
        int resCount = 0;

        for (int i=0; i < entries.Length; i++)
        {
            if (entries[i].Message.ToLower().Contains(text.ToLower()))
            {
                temproraryResult[resCount] = entries[i];
                resCount++;
            }
        }
        LogEntry[] res = new LogEntry[resCount];
        Array.Copy(temproraryResult, res, resCount);
        return res;
    }

    public static int CountByLevel(LogEntry[] entries, string level)
    {
        int resCount = 0;

        for (int i=0; i < entries.Length; i++)
        {
            if (entries[i].Level.ToLower() == level.ToLower())
            {
                resCount++;
            }
        }

        return resCount;
    }

    public static string GetServerStatus(LogEntry[] entries)
    {
        bool FatalServer = false;
        bool Error = false;

        for (int i=0; i < entries.Length; i++)
        {
            if (entries[i].Level.ToLower() == "fatal" && entries[i].Category.ToLower() == "server")
            {
                FatalServer = true;
                break;
            }
            if (entries[i].Level.ToLower() == "error")
            {
                Error = true;
            }
        }
        if (FatalServer)
        {
            return "КРИТИЧЕСКАЯ ОШИБКА";
        }
        if (Error)
        {
            return "Присутствуют ошибки";
        }
        return "ошибок нет";
    }
}
