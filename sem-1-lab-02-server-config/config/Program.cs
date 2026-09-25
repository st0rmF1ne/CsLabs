using System.Runtime.Intrinsics.Arm;

namespace lab02;


public class Program
{
    public static void Main()
    {
        Console.WriteLine("Запуск сервера");
        Console.WriteLine("Доступное кол-во оперативной памяти: ");
        int ram = int.Parse(Console.ReadLine());
        Console.WriteLine("Кол-во игроков: ");
        int Players = int.Parse(Console.ReadLine());
        bool Public = true;
        Console.WriteLine("Установить пароль?(Y/N) ");
        bool hasPassword = Console.ReadLine() == "Y";

        Console.WriteLine(CheckConfiguration(Players, ram, Public, hasPassword));

    }
    public static string CheckConfiguration(int Players, int ram, bool Public, bool hasPassword)
    {
        if (Players <= 0)
        {
            return "Запуск невозможен: количество игроков должно быть больше нуля.";
        }

        if (ram < 2)
        {
            return "Запуск невозможен: серверу недостаточно оперативной памяти.";
        }

        if (Public && hasPassword)
        {
            return "Запуск возможен с предупреждением: публичный сервер защищён паролем.";
        }

        if (Players > ram * 25)
        {
            return "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти.";
        }

        return "Сервер готов к запуску.";
    }
}
    
    
    