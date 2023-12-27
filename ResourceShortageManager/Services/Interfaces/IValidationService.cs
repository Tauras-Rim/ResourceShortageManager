namespace ResourceShortageManager.Services.Interfaces;

public interface IValidationService
{
    public void Validate(Func<bool> condition, string message);
}