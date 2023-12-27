using System.Text.Json;
using ResourceShortageManager.Services.Interfaces;

namespace ResourceShortageManager.Services;

public class SerializerService : ISerializerService
{
    public void AddItemToJson<T>(T item, string fileName)
    {
        var list = GetListFromJson<T>(fileName);
        
        list.Add(item);

        WriteListToJson(list, fileName);
    }

    //TODO: ar reik daugiau kazka catchint?
    public List<T> GetListFromJson<T>(string fileName)
    {
        var filePath = Path.Combine("..", "..", "..", "Storage", fileName);
        string fileData;
        
        try
        {
            fileData = File.ReadAllText(filePath);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("Exception occured while getting item list from file");
            throw e;
        }

        return string.IsNullOrWhiteSpace(fileData) ? Enumerable.Empty<T>().ToList() : JsonSerializer.Deserialize<List<T>>(fileData);
    }

    public void RemoveItemFromJson<T>(string fileName, Predicate<T> equalityPredicate)
    {
        var list = GetListFromJson<T>(fileName);
        
        var indexToRemove = list.FindIndex(equalityPredicate);

        list.RemoveAt(indexToRemove);
        
        WriteListToJson(list, fileName);
    }

    private static void WriteListToJson<T>(List<T> list, string fileName)
    {
        var filePath = Path.Combine("..", "..", "..", "Storage", fileName);
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var newJson = JsonSerializer.Serialize(list, options);
        
        File.WriteAllText(filePath, newJson);
    }
}