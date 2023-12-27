using System.Text.Json;
using ResourceShortageManager.Services;

namespace ResourceShortageManagerTests
{
    [TestFixture]
    public class SerializerServiceTests
    {
        private SerializerService? _serializerService;
        private const string FileName = "test.json";
        private readonly string _filePath = Path.Combine("..", "..", "..", "Storage", FileName);


        [SetUp]
        public void SetUp()
        {
            _serializerService = new SerializerService();
        }

        [Test]
        public void AddItemToJson_ValidInput_AddsItemToJson()
        {
            // Arrange
            var itemToAdd = new TestItem { Id = 1, Name = "Item1" };

            // Act
            _serializerService.AddItemToJson(itemToAdd, FileName);

            // Assert
            var items = _serializerService.GetListFromJson<TestItem>(FileName);

            CollectionAssert.Contains(items, itemToAdd);
        }


        [Test]
        public void GetListFromJson_ListIsNotEmpty_ReturnsList()
        {
            // Arrange
            var testList = new List<TestItem>
            {
                new() { Id = 1, Name = "Item1" },
                new() { Id = 2, Name = "Item2" }
            };
            
            File.WriteAllText(_filePath, JsonSerializer.Serialize(testList));

            // Act
            var items = _serializerService.GetListFromJson<TestItem>(FileName);

            // Assert
            Assert.That(items, Is.EquivalentTo(testList));
        }
        
        [Test]
        public void GetListFromJson_FileIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var testList = new List<TestItem>();
            
            File.WriteAllText(_filePath, "");

            // Act
            var items = _serializerService.GetListFromJson<TestItem>(FileName);

            // Assert
            Assert.That(items, Is.EquivalentTo(testList));
        }

        [Test]
        public void RemoveItemFromJson_ItemExists_RemovesItemFromJson()
        {
            // Arrange
            var testData = new List<TestItem>
            {
                new() { Id = 1, Name = "Item1" },
                new() { Id = 2, Name = "Item2" }
            };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(testData));

            // Act
            _serializerService.RemoveItemFromJson<TestItem>(FileName, item => item.Id == 1);

            // Assert
            var remainingItems = _serializerService.GetListFromJson<TestItem>(FileName);
            Assert.That(remainingItems, Has.Count.EqualTo(1));
            Assert.That(remainingItems.Exists(item => item.Id == 2));
        }
        
    }

    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otherItem = (TestItem)obj;
            return Id == otherItem.Id && Name == otherItem.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
