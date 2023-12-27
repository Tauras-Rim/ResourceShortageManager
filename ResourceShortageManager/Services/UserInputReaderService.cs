using ResourceShortageManager.Services.Interfaces;

namespace ResourceShortageManager.Services;

public class UserInputReaderService : IUserInputReaderService
{
    public string ReadLine()
    {
        return Console.ReadLine();
    }
}