using System.Runtime.CompilerServices;

namespace generator;

public class Program
{
    public static string EventStart(string[] lines)
    {
        string startTime = "";
        bool EventIsRunning = false;
        List<string> EventLines = new List<string>();
        foreach(string line in lines)
        {
            if (line.Contains("Событие началось:"))
            {
                EventIsRunning = true;
                int index = line.IndexOf(' ',(' ') + 1);
                startTime = line.Substring(0, index);

            }
            else if (line.Contains("Событие \"Восстание Ледяного Пламени\" закрыто"))
            {
                EventIsRunning = false;
            }
        }
        return startTime;
    }

    public static string Winner(string[] lines)
    {
        bool EventIsRunning = false;
        string winner = "";
        foreach (string line in lines)
        {
            if (line.Contains("Событие началось:"))
            {
                EventIsRunning = true;
                int messageStart = line.IndexOf("] ") + 2;
                int winnerEnd = line.IndexOf(" объявлены победителями");
                winner = line.Substring(messageStart, winnerEnd - messageStart);

            }
            else if (line.Contains("Событие \"Восстание Ледяного Пламени\" закрыто"))
            {
                EventIsRunning = false;
            }
        }
        return winner;
    }

    public static int Points(string[] lines)
    {
        bool EventIsRunning = false;
        int points = 0;
        foreach (string line in lines)
        {
            if (line.Contains("Событие началось:"))
            {
                EventIsRunning = true;
                const string startText = "Ночные совы получили ";
                int start = line.IndexOf((startText) + startText.Length);
                int end = line.IndexOf(" очков", start);
                points = int.Parse(line.Substring(start, end - start));
            }
            else if (line.Contains("Событие \"Восстание Ледяного Пламени\" закрыто"))
            {
                EventIsRunning = false;
            }
        }

        return points;
    }
    public static string Loot(string[] lines)
    {
        string loot = "";
        foreach (string line in lines)
        {
            int index = line.IndexOf("Ночные совы получили ивентовый предмет:");
            loot = line.Substring(index, line.Length - index);
        }
        return loot;
    }

    public static int Warnings(string[] lines)
    {
        int warningCount = 0;
        foreach (string line in lines)
        {
            if (line.Contains("[Warning]"))
            {
                warningCount++;
            }
        }
        return warningCount;
    }

    public static int Error(string[] lines)
    {
        int errorCount = 0;
        foreach (string line in lines)
        {
            if (line.Contains("[Error]"))
            {
                errorCount++;
            }
        }
        return errorCount;
    }
    public static string BuildReport(string[] lines)
    {
        return "Итоги:\n" +
            $"Дата: {EventStart}\n" +
            $"Победитель: {Winner}\n" +
            $"Очки победителя: {Points}\n" +
            $"Ивентовый предмет: {Loot}\n" +
            $"Предупреждений во время события: {Warnings}\n" +
            $"Ошибок во время события: {Error}";
    }
    public static void Main()
    {
        string[] lines = File.ReadAllLines("event_server.log");
        Console.WriteLine(BuildReport(lines));
    }

    
    
}


