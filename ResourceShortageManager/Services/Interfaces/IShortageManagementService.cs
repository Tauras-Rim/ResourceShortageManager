using ResourceShortageManager.Models;

namespace ResourceShortageManager.Services.Interfaces;

public interface IShortageManagementService
{
    public void RegisterNewShortage(UserModel user);

    public void DeleteRequest(UserModel user);

    public void ListAllRequests(UserModel user);
}