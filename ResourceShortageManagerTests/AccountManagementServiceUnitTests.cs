using NSubstitute;
using ResourceShortageManager;
using ResourceShortageManager.Models;
using ResourceShortageManager.Services;
using ResourceShortageManager.Services.Interfaces;

namespace ResourceShortageManagerTests
{
    [TestFixture]
    public class AccountManagementServiceTests
    {
        private ISerializerService _serializerService;
        private IValidationService _validationService;
        private IUserInputReaderService _userInputReaderService;
        private AccountManagementService _accountService;

        [SetUp]
        public void SetUp()
        {
            _serializerService = Substitute.For<ISerializerService>();
            _validationService = Substitute.For<IValidationService>();
            _userInputReaderService = Substitute.For<IUserInputReaderService>();
            _accountService =
                new AccountManagementService(_serializerService, _validationService, _userInputReaderService);
        }
        
        [Test]
        public void Login_InvalidUserId_ReturnsNull()
        {
            // Arrange
            const int userId = 1;
            var userList = new List<UserModel>
            {
                new() { UserId = 1, IsAdministrator = true }
            };
            
            var conditionDelegate = Substitute.For<Func<bool>>();
            conditionDelegate().Returns(false);

            _userInputReaderService.ReadLine().Returns(0.ToString());

            _serializerService.GetListFromJson<UserModel>("Users.json").Returns(callInfo => userList.ToList());

            _validationService.Validate(conditionDelegate, Arg.Any<string>());

            _accountService =
                new AccountManagementService(_serializerService, _validationService, _userInputReaderService);
            // Act
            var result = _accountService.Login();

            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void RegisterUser_InvalidUserId_DoesNotAddUser()
        {
            // Arrange
            var userList = new List<UserModel>
            {
                new() { UserId = 0, IsAdministrator = true }
            };

            _serializerService.GetListFromJson<UserModel>("Users.json").Returns(callInfo => userList.ToList());
            
            // Act
            _accountService.RegisterUser();

            // Assert
            _serializerService.DidNotReceive().AddItemToJson(Arg.Any<UserModel>(), Arg.Any<string>());
        }
        
        [Test]
        public void DeleteCurrentUser_UserExists_RemovesUserFromJson()
        {
            // Arrange
            var userToDelete = new UserModel { UserId = 1 };
            var userList = new List<UserModel>
            {
                userToDelete,
                new() { UserId = 2 }
            };
        
            _serializerService.GetListFromJson<UserModel>("Users.json").Returns(userList);
        
            // Act
            _accountService.DeleteCurrentUser(userToDelete);
        
            // Assert
            _serializerService.Received(1).RemoveItemFromJson("Users.json", Arg.Any<Predicate<UserModel>>());
        }
    }
}
