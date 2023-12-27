namespace ResourceShortageManager.Services.Interfaces;

public interface ISerializerService
{
    public void AddItemToJson<T>(T item, string fileName);

    public List<T> GetListFromJson<T>(string fileName);

    public void RemoveItemFromJson<T>(string fileName, Predicate<T> equalityPredicate);
}