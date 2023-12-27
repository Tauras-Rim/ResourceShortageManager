using ResourceShortageManager.Models;
using ResourceShortageManager.Services.Interfaces;

namespace ResourceShortageManager.Services;

public class ShortageManagementService : IShortageManagementService
{
    private readonly ISerializerService _serializerService;
    private readonly IValidationService _validationService;
    private readonly IUserInputReaderService _userInputReaderService;
    
    private const string FileName = "ResourceShortage.json";
    private const string ErrorMessage = "Incorrect input, try again!";
    private const string FilterErrorMessage = "This filter has already been added";

    public ShortageManagementService(ISerializerService serializerService, IValidationService validationService, IUserInputReaderService userInputReaderService)
    {
        _serializerService = serializerService;
        _validationService = validationService;
        _userInputReaderService = userInputReaderService;
    }

    public void RegisterNewShortage(UserModel user)
    {
        Console.WriteLine("Enter Your title");
        var title = _userInputReaderService.ReadLine();
        
        Console.WriteLine("Enter Your room");
        Console.WriteLine("1 - Meeting room");
        Console.WriteLine("2 - Kitchen");
        Console.WriteLine("3 - Bathroom");
        byte roomByte = 0;
        
        _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out roomByte) || roomByte < 1 || roomByte > 3, ErrorMessage);

        var room = (RoomEnum.Room)Convert.ToByte(roomByte);
        
        Console.WriteLine("Enter the category");
        Console.WriteLine("1 - Electronics");
        Console.WriteLine("2 - Food");
        Console.WriteLine("3 - Other");
        byte categoryByte = 0;

        _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out categoryByte) || categoryByte < 1 || categoryByte > 3, ErrorMessage);
        
        var category = (CategoryEnum.Category)categoryByte;
        
        Console.WriteLine("Enter the priority level (1-10)");
        byte priorityByte = 0;
        
        _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out priorityByte) || priorityByte < 1 || priorityByte > 10, ErrorMessage);
        
        var priority = Convert.ToByte(priorityByte);
        
        var newShortage = new ShortageModel
        {
            Id = GenerateId(),
            UserId = user.UserId,
            Title = title,
            Name = user.Name,
            Room = room,
            Category = category,
            Priority = priority,
            CreatedOn = DateTime.Now.Date
        };

        var shortageList = _serializerService.GetListFromJson<ShortageModel>(FileName);
        
        CheckForDuplicateRequest(shortageList, newShortage);
    }

    public void DeleteRequest(UserModel user)
    {
        var shortageList = _serializerService.GetListFromJson<ShortageModel>(FileName);
        
        if (!user.IsAdministrator)
            shortageList = shortageList.Where(s => s.UserId == user.UserId).ToList();
        
        if (IsListEmpty(shortageList))
        {
            return;
        }
        
        Console.WriteLine("Which request do You want to remove? (enter request number)");
            
        PrintList(shortageList);

        var itemToRemoveIndex = -1;
            
        _validationService.Validate(() => !int.TryParse(_userInputReaderService.ReadLine(), out itemToRemoveIndex) || itemToRemoveIndex < 1 || itemToRemoveIndex > shortageList.Count, "request not found");
            
        itemToRemoveIndex--;
        
        var itemToRemove = shortageList.ElementAt(itemToRemoveIndex);

        var equalityPredicate = new Predicate<ShortageModel>(s => s.Id == itemToRemove.Id);
            
        _serializerService.RemoveItemFromJson(FileName, equalityPredicate);
    }

    public void ListAllRequests(UserModel user)
    {
        var shortageList = _serializerService.GetListFromJson<ShortageModel>(FileName);

        if (!user.IsAdministrator)
            shortageList = shortageList.Where(s => s.UserId == user.UserId).ToList();
        
        if (IsListEmpty(shortageList))
        {
            return;
        }
        
        byte filterChoice = 0;
        var appliedFilters = new Dictionary<string, string>();
        
        var availableFilters = new List<string>
        {
            "title",
            "room",
            "category",
            "from date",
            "end date"
        };
        
        while (filterChoice != 6)
        {
            PrintAppliedFilters(appliedFilters);

            PrintAvailableFilters(availableFilters, appliedFilters);

            _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out filterChoice) || filterChoice < 1 || filterChoice > 6, ErrorMessage);

            switch (filterChoice)
            {
                case 1:
                    if (!appliedFilters.ContainsKey("title"))
                    {
                        appliedFilters["title"] = ApplyTitleFilter(shortageList);
                    }
                    else
                    {
                        Console.WriteLine(FilterErrorMessage);
                    }
                    break;
                
                case 2:
                    if (!appliedFilters.ContainsKey("room"))
                    {
                        appliedFilters["room"] = ApplyRoomFilter(shortageList);
                    }
                    else
                    {
                        Console.WriteLine(FilterErrorMessage);
                    }
                    break;
                
                case 3:
                    if (!appliedFilters.ContainsKey("category"))
                    {
                        appliedFilters["category"] = ApplyCategoryFilter(shortageList);
                    }
                    else
                    {
                        Console.WriteLine(FilterErrorMessage);
                    }
                    break;
                
                case 4:
                    if (!appliedFilters.ContainsKey("from date"))
                    {
                        appliedFilters["from date"] = ApplyFromDateFilter(shortageList);
                    }
                    else
                    {
                        Console.WriteLine(FilterErrorMessage);
                    }
                    break;
                
                case 5:
                    if (!appliedFilters.ContainsKey("end date"))
                    {
                        appliedFilters["end date"] = ApplyEndDateFilter(shortageList);
                    }
                    else
                    {
                        Console.WriteLine(FilterErrorMessage);
                    }
                    break;
                
                case 6:
                    shortageList = shortageList.OrderByDescending(s => s.Priority).ToList();
                    PrintList(shortageList);
                    break;
            }
        }
    }

    private static void PrintAppliedFilters(Dictionary<string, string> appliedFilters)
    {
        Console.WriteLine("Currently applied filters:");

        foreach (var filter in appliedFilters)
        {
            Console.WriteLine($"{filter.Key}: {filter.Value}");
        }
    }

    private static void PrintAvailableFilters(IReadOnlyCollection<string> availableFilters, IReadOnlyDictionary<string, string> appliedFilters)
    {
        for (var i = 0; i < availableFilters.Count; i++)
        {
            if (!appliedFilters.ContainsKey(availableFilters.ElementAt(i)))
            {
                Console.WriteLine($"{i + 1} - Filter by {availableFilters.ElementAt(i)}");
            }
        }
        Console.WriteLine("6 - Print requests");
    }

    private string ApplyCategoryFilter(List<ShortageModel> shortageList)
    {
        Console.WriteLine("Which category do You want to filter by?");
        Console.WriteLine("1 - Electronics");
        Console.WriteLine("2 - Food");
        Console.WriteLine("3 - Other");
        byte categoryFilter = 0;
        
        _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out categoryFilter) || categoryFilter < 1 || categoryFilter > 3, ErrorMessage);
        
        shortageList.RemoveAll(s => s.Category != (CategoryEnum.Category)categoryFilter);

        return ((CategoryEnum.Category)categoryFilter).ToString();
    }

    private string ApplyRoomFilter(List<ShortageModel> shortageList)
    {
        Console.WriteLine("Which room do You want to filter by?");
        Console.WriteLine("1 - Meeting room");
        Console.WriteLine("2 - Kitchen");
        Console.WriteLine("3 - Bathroom");
        byte roomFilter = 0;
        
        _validationService.Validate(() => !byte.TryParse(_userInputReaderService.ReadLine(), out roomFilter) || roomFilter < 1 || roomFilter > 3, ErrorMessage);

        shortageList.RemoveAll(s => s.Room != (RoomEnum.Room)roomFilter);
        
        return ((RoomEnum.Room)roomFilter).ToString();
    }
    
    private string ApplyEndDateFilter(List<ShortageModel> shortageList)
    {
        Console.WriteLine("Enter Your desired end date filter (yyyy-mm-dd)");
        var endDateFilter = DateTime.MaxValue;
        
        _validationService.Validate(() => !DateTime.TryParse(_userInputReaderService.ReadLine(), out endDateFilter), ErrorMessage);

        shortageList.RemoveAll(s => s.CreatedOn > endDateFilter);
        
        return endDateFilter.ToString("yyyy-MM-dd");
    }
    
    private string ApplyFromDateFilter(List<ShortageModel> shortageList)
    {
        Console.WriteLine("Enter Your desired from date filter (yyyy-mm-dd)");
        var fromDateFilter = DateTime.MinValue;
        
        _validationService.Validate(() => !DateTime.TryParse(_userInputReaderService.ReadLine(), out fromDateFilter), ErrorMessage);

        shortageList.RemoveAll(s => s.CreatedOn < fromDateFilter);
        
        return fromDateFilter.ToString("yyyy-MM-dd");
    }

    private string ApplyTitleFilter(List<ShortageModel> shortageList)
    {
        Console.WriteLine("Enter Your desired title filter");
        var titleFilter = _userInputReaderService.ReadLine();
        
        shortageList.RemoveAll(s => !s.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase));
        
        return titleFilter;
    }

    private void OverrideRequestByPriority(ShortageModel newShortage, ShortageModel initialShortage)
    {
        if (newShortage.Priority > initialShortage.Priority)
        {
            var equalityPredicate = new Predicate<ShortageModel>(i => i.Id == initialShortage.Id);
            
            _serializerService.RemoveItemFromJson(FileName, equalityPredicate);
            _serializerService.AddItemToJson(newShortage, FileName);
            
            Console.WriteLine("Shortage has been overriden");
        }
        else
        {
            Console.WriteLine("The new shortage has lower priority, therefore it won't override the old one");
        }
    }

    private static void PrintList(IReadOnlyList<ShortageModel> list)
    {
        if (!list.Any())
        {
            Console.WriteLine("There are no requests matching these filters");
            return;
        }
        
        Console.WriteLine("Results:");
        for (var i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"Item: {i + 1}");
            Console.WriteLine(list[i].ToString());
        }
    }

    private bool IsListEmpty(List<ShortageModel> list)
    {
        if (!list.Any())
        {
            Console.WriteLine("There are no requests");
            return true;
        }
        return false;
    }
    
    private static int GenerateId()
    {
        return new Random().Next();
    }
    
    private void CheckForDuplicateRequest(List<ShortageModel> shortageList, ShortageModel newShortage)
    {
        if (shortageList.Any(s => s.Title == newShortage.Title && s.Room == newShortage.Room))
        {
            Console.WriteLine("Warning: this shortage has already been added!");

            var initialShortage =
                shortageList.Single(s => s.Title == newShortage.Title && s.Room == newShortage.Room);
                    
            OverrideRequestByPriority(newShortage, initialShortage);
        }
        else
        {
            _serializerService.AddItemToJson(newShortage, FileName);
        }
    }
}