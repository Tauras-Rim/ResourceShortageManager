using ResourceShortageManager.Services.Interfaces;

namespace ResourceShortageManager.Services;

public class ValidationService : IValidationService
{
    public void Validate(Func<bool> condition, string message)
    {
        while (condition())
        {
            Console.WriteLine(message);
        }
    }
}