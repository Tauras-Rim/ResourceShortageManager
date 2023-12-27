using Microsoft.Extensions.DependencyInjection;
using ResourceShortageManager.Models;
using ResourceShortageManager.Services;
using ResourceShortageManager.Services.Interfaces;

var serviceProvider = new ServiceCollection()
    .AddScoped<IShortageManagementService, ShortageManagementService>()
    .AddScoped<ISerializerService, SerializerService>()
    .AddScoped<IAccountManagementService, AccountManagementService>()
    .AddScoped<IValidationService, ValidationService>()
    .AddScoped<IUserInputReaderService, UserInputReaderService>()
    .BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var shortageManagement = scope.ServiceProvider.GetRequiredService<IShortageManagementService>();
    var accountManagementService = scope.ServiceProvider.GetRequiredService<IAccountManagementService>();
    var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
    var userInputReaderService = scope.ServiceProvider.GetRequiredService<IUserInputReaderService>();

    UserModel user = null;

    while (user == null)
    {
        Console.WriteLine("Do You want to login or register?");
        Console.WriteLine("1 - Register");
        Console.WriteLine("2 - Login");
        byte accountChoice = 0;
        
        validationService.Validate(() => !byte.TryParse(userInputReaderService.ReadLine(), out accountChoice) || accountChoice < 1 || accountChoice > 2, "Incorrect input, try again!");
        
        switch (accountChoice)
        {
            case 1:
                accountManagementService.RegisterUser();
                break;
            case 2:
                user = accountManagementService.Login();
                break;
        }
    }

    byte actionChoice = 0;
    
    while (true)
    {
        Console.WriteLine("What do You want to do?");
        Console.WriteLine("1 - Register a new shortage");
        Console.WriteLine("2 - Delete a shortage request");
        Console.WriteLine("3 - List shortage requests");
        Console.WriteLine("4 - Delete current user and quit");
        Console.WriteLine("5 - Quit");
        
        validationService.Validate(() => !byte.TryParse(userInputReaderService.ReadLine(), out actionChoice) || actionChoice < 1 || actionChoice > 5, "Incorrect input, try again!");
        
        switch (actionChoice)
        {
            case 1:
                shortageManagement.RegisterNewShortage(user);
                break;
            case 2:
                shortageManagement.DeleteRequest(user);
                break;
            case 3:
                shortageManagement.ListAllRequests(user);
                break;
            case 4:
                accountManagementService.DeleteCurrentUser(user);
                Environment.Exit(0);
                break;
            case 5:
                Environment.Exit(0);
                break;
        }
    }
}