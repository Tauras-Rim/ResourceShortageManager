using System.Text.Json.Serialization;

namespace ResourceShortageManager.Models;

public class ShortageModel
{
    public int Id { get; init; }
    public uint UserId { get; init; }
    public string? Title { get; init; }
    public string? Name { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomEnum.Room Room { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CategoryEnum.Category Category { get; init; }
    public byte Priority { get; init; }
    public DateTime CreatedOn { get; init; }

    public override string ToString()
    {
        return $"ID: {Id}\nTitle: {Title}\nName: {Name}\nRoom: {Room}\nCategory: {Category}\nPriority: {Priority}\nCreated: {CreatedOn}\n";
    }
}