using ResourceShortageManager.Services;

namespace ResourceShortageManagerTests;

public class ValidationUnitTests
{
    private ValidationService _validationService;

    [SetUp]
    public void SetUp()
    {
        _validationService = new ValidationService();
    }
    
    [Test]
    public void Validate_WithFalseCondition_NoErrorMessage()
    {
        // Arrange
        const string errorMessage = "Error message";
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        _validationService.Validate(() => false, errorMessage);

        // Assert
        Assert.That(consoleOutput.ToString().Trim(), Is.EqualTo(""));
    }
}