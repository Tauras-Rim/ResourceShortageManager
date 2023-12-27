using ResourceShortageManager.Models;
using ResourceShortageManager.Services.Interfaces;

namespace ResourceShortageManager.Services;

public class AccountManagementService : IAccountManagementService
{
    private readonly ISerializerService _serializerService;
    private readonly IValidationService _validationService;
    private readonly IUserInputReaderService _userInputReaderService;
    
    private const string ErrorMessage = "Incorrect input, try again!";
    private const string FileName = "Users.json";

    public AccountManagementService(ISerializerService serializerService, IValidationService validationService, IUserInputReaderService userInputReaderService)
    {
        _serializerService = serializerService;
        _validationService = validationService;
        _userInputReaderService = userInputReaderService;
    }
    public UserModel Login()
    {
        Console.WriteLine("Enter Your user ID");
        var userId = -1;
        
        _validationService.Validate(() => !int.TryParse(_userInputReaderService.ReadLine(), out userId), ErrorMessage);

        var userList = _serializerService.GetListFromJson<UserModel>(FileName);

        if (userList.All(u => u.UserId != userId))
        {
            Console.WriteLine($"User with id: {userId} doesn't exist");
            return null;
        }

        var user = userList.Single(u => u.UserId == userId);

        Console.WriteLine(user.IsAdministrator
            ? $"You have logged in as administrator with ID: {userId}"
            : $"You have logged in as user with id: {userId}");

        return userList.Single(u => u.UserId == userId);
    }

    public void RegisterUser()
    {
        Console.WriteLine("Enter Your name");
        var userName = _userInputReaderService.ReadLine();
        
        Console.WriteLine("Enter the user id you want to be identified by");
        uint userId = 0;
        
        _validationService.Validate(() => !uint.TryParse(_userInputReaderService.ReadLine(), out userId), ErrorMessage);
        
        var userList = _serializerService.GetListFromJson<UserModel>(FileName);

        if (userList.Any(user => user.UserId == userId))
        {
            Console.WriteLine("This ID already exists");
            return;
        }
        
        Console.WriteLine("Is this user an administrator?");
        Console.WriteLine("1 - yes");
        Console.WriteLine("2 - no");
        byte adminChoice = 0;
        
        _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out adminChoice) || adminChoice < 1 || adminChoice > 2, ErrorMessage);

        var isAdmin = adminChoice switch
        {
            1 => true,
            2 => false,
            _ => false
        };

        var user = new UserModel
        {
            Name = userName,
            UserId = userId,
            IsAdministrator = isAdmin
        };
        
        _serializerService.AddItemToJson(user, FileName);
    }

    public void DeleteCurrentUser(UserModel user)
    {
        var equalityPredicate = new Predicate<UserModel>(u => u.UserId == user.UserId);
        
        _serializerService.RemoveItemFromJson(FileName, equalityPredicate);
    }
}