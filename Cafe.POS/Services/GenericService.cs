using System.Text.Json;

namespace Cafe.POS.Services;

public class GenericService<T> where T : class
{
    // <summary>
    /// Generic service class for performing common operations on entities of type T.
    /// </summary>
    public static List<T> GetAll(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        var json = File.ReadAllText(filePath);

        var result = JsonSerializer.Deserialize<List<T>>(json);
        
        return result ?? [];
    }

    public  static void SaveAll(List<T> entity, string directoryPath, string filePath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var json = JsonSerializer.Serialize(entity);

        File.WriteAllText(filePath, json);
    }
}