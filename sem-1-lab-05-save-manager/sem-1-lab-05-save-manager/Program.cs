using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Xml.Linq;
namespace manager;

public class SaveEntry
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("file")]
    public string File { get; set; }
    [JsonProperty("UpdateAt")]
    public DateTime UpdateAt { get; set; }
}
public class Position
{
    public float X { get; set; }
    public float Y { get; set; }
}
public class SaveData
{
    public string Character { get; set; }
    public string Map { get; set; }
    public Position Position { get; set; }
    public int Health { get; set; }
    public List<string> Inventory { get; set; }
}
internal class Program
{

    public static List<SaveEntry> LoadIndex(string indexPath)
    {
        if(!File.Exists(indexPath))
        {
            Console.WriteLine("Файл не найден");
            return null;
        }
        try
        {
            string json = File.ReadAllText(indexPath);
            List<SaveEntry> entries = JsonConvert.DeserializeObject<List<SaveEntry>>(json);
            return entries;
        }
        catch (JsonException)
        {
            Console.WriteLine("Файл поврежден");
            return null;
        }
    }

    public static SaveData LoadSave(string savesDirectory, SaveEntry entry)
    {
        if (!File.Exists(savesDirectory))
        {
            return null;
        }
        try
        {
            string json = File.ReadAllText(savesDirectory);
            SaveData save = JsonConvert.DeserializeObject<SaveData>(json);
            return save;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static void PrintSave(SaveEntry entry, SaveData save)
    {
        if (entry!=null)
        {
            Console.WriteLine($"Слот: {entry.Name}");
            Console.WriteLine($"");

        }
        if (save!=null)
        {
            Console.WriteLine($"Персонаж: {save.Character}");
            Console.WriteLine($"Карта: {save.Map}");

            if(save.Position!=null)
            {
                Console.WriteLine($"Координаты: X = {save.Position.X} Y = {save.Position.Y}");
            }
            Console.WriteLine($"Здоровье: {save.Health}");

            if (save.Inventory != null)
            {
                Console.WriteLine($"Инвентарь: {string.Join(", ", save.Inventory)}");
            }
            else
            {
                Console.WriteLine("Инвентарь пуст");
            }
        }
    }


    public static SaveEntry FindByName(List<SaveEntry> entries, string name)
    {
        foreach (SaveEntry z in entries)
        {
            if (z.Name != null && z.Name.Contains(name))
            {
                return z;
            }
        }
        return null;
    }

    public static void SaveGame(string savesDirectory, SaveEntry entry, SaveData save)
    {
        try
        {
            string json = JsonConvert.SerializeObject(save, Formatting.Indented);
            File.WriteAllText(Path.Combine(savesDirectory, entry.File), json);
        }
        catch (IOException)
        {
            Console.WriteLine("Ошибка");
        }
    }

    public static void SaveIndex(string indexPath, List<SaveEntry> entries)
    {
        try
        {
            string json = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(indexPath, json);
        }
        catch(IOException)
        {
            Console.WriteLine("Ошибка");
        }
    }

    public static void CreateBackup(string savesDirectory, string backupsDirectory, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.File))
        {
            return;
        }
        string sourcepath = Path.Combine(savesDirectory, entry.File);
        string destpath = Path.Combine(backupsDirectory, entry.File);
        try
        {
            Directory.CreateDirectory(backupsDirectory);
            File.Copy(sourcepath, destpath, true);
            Console.WriteLine("Успешная резервная копия");
        }
        catch(IOException)
        {
            Console.WriteLine("Ошбика не удалось создать резервную копию");
        }
    }

    public static void RestoreBackup(string savesDirectory, string backupsDirectory, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.File))
        {
            return;
        }
        string backuppath = Path.Combine(savesDirectory, entry.File);
        string targetpath = Path.Combine(backupsDirectory, entry.File);

        try
        {
            Directory.CreateDirectory(backupsDirectory);
            File.Copy(backuppath, targetpath, true);
            Console.WriteLine("Сохранение успешно востановлено из резервной копии");
        }
        catch (IOException)
        {
            Console.WriteLine("Ошбика не удалось восстановить сохранение");
        }

    }

    public static void DeleteSave(string savesDirectory, string indexPath, List<SaveEntry> entries, SaveEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.File))
        {
            return;
        }
        string fulepath = Path.Combine(savesDirectory, entry.File);
        if (File.Exists(fulepath))
        {
            try
            {
                File.Delete(fulepath);
                Console.WriteLine("Сохранение успешно удалено");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка удаления сохранения");

            }

        }
        else
        {
            Console.WriteLine("Файл сохранения отсутсвует");
        }
        entries.Remove(entry);
        try
        {
            string updatedJson = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(indexPath, updatedJson);

        }
        catch
        {
            Console.WriteLine("ошибка обновления индекса");
        }
    }


    public static void Main()
    {
        string backupsDirectory = "backups";
        string savesDirectory = "saves";
        string indexPath = "index.json";
        List<SaveEntry> indexList = LoadIndex(indexPath);

        while (true)
        {
            Console.WriteLine("\n====== Меню управления сохранениями ======");
            Console.WriteLine("1. Показать список сохранений");
            Console.WriteLine("2. Посмотреть данные сохранения");
            Console.WriteLine("3. Изменить данные персонажа и записать их обратно");
            Console.WriteLine("4. Создать резервную копию");
            Console.WriteLine("5. Восстановить резервную копию");
            Console.WriteLine("6. Удалить сохранение");
            Console.WriteLine("7. Выйти");
            Console.Write("Выберите пункт меню (1-7): ");

            string dilemma = Console.ReadLine();
            if (dilemma == "7") break;

            if (dilemma == "1")
            {
                Console.WriteLine("\n Список сохранений:");
                foreach (var entry in indexList)
                {
                    Console.WriteLine($"Слот {entry.Name} | Файл {entry.File} | Изменён {entry.UpdateAt}");
                }
                continue;
            }

            Console.WriteLine("Введите имя сохранения");
            string huisyournow = Console.ReadLine();
            SaveEntry foundentry = FindByName(indexList, huisyournow);
            if (foundentry == null)
            {
                Console.WriteLine("Имя не найдено");
                continue;
            }

            switch(dilemma)
            {
                case "2":
                    {
                        SaveData save = LoadSave(savesDirectory, foundentry);
                        PrintSave(foundentry, save);
                        break;
                    }

                case "3":
                    {
                        SaveData save = LoadSave(savesDirectory, foundentry);
                        if (save!= null)
                        {
                            Console.WriteLine("\n Редактирование сохранения");

                            Console.WriteLine($"Текущая карта: {save.Map} Новая карта:");
                            string newmap = Console.ReadLine();
                            if (string.IsNullOrEmpty(newmap))
                            {
                                save.Map = newmap;
                            }

                            Console.WriteLine($"\n Текущее здоровье {save.Health} Новое здоровье:");
                            string newhp = Console.ReadLine();
                            if (int.TryParse(newhp, out int newHealth))
                            {
                                save.Health = newHealth;
                            }

                            if (save.Position == null) save.Position = new Position();

                            Console.WriteLine($"Текущий Х:{save.Position.X} Новый X:");
                            string newx = Console.ReadLine();
                            if (float.TryParse(newx, out float newX))
                            {
                                save.Position.X = newX;
                            }

                            Console.WriteLine($"Текущий Y:{save.Position.Y} Новый Y:");
                            string newy = Console.ReadLine();
                            if (float.TryParse(newy, out float newY))
                            {
                                save.Position.Y = newY;
                            }

                            string currentInv = save.Inventory != null ? string.Join(", ", save.Inventory) : "пусто";
                            Console.Write($"Текущий инвентарь [{currentInv}]. Новый инвентарь через запятую: ");
                            string inputInv = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(inputInv))
                            {
                                var items = inputInv.Split(',');
                                save.Inventory = new List<string>();
                                foreach (var item in items)
                                {
                                    save.Inventory.Add(item.Trim());
                                }
                            }
                            SaveGame(savesDirectory, foundentry, save);

                            foundentry.UpdateAt = DateTime.Now;

                            SaveIndex(indexPath, indexList);

                            Console.WriteLine("Данные успешно обновлены и сохранены!");
                        }
                        break;

                    }
                
                case "4":
                    {
                        CreateBackup(savesDirectory, backupsDirectory, foundentry);
                        break;
                    }
                case "5":
                    {
                        RestoreBackup(savesDirectory, backupsDirectory, foundentry);
                        break;
                    }
                case "6":
                    {
                        DeleteSave(savesDirectory, indexPath, indexList, foundentry);
                        break;
                    }
                default:
                    Console.WriteLine("Неверный выбор пункта меню.");
                    break;
            }
        }

    }
}