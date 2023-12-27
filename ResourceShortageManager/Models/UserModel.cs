namespace ResourceShortageManager.Models;

public class UserModel
{
    public uint UserId { get; init; }
    public string? Name { get; init; }
    public bool IsAdministrator { get; init; }
}