using ResourceShortageManager.Models;

namespace ResourceShortageManager.Services.Interfaces;

public interface IAccountManagementService
{
    public UserModel Login();
    public void RegisterUser();

    public void DeleteCurrentUser(UserModel user);
}