using TodoItems.Core.DuedateStartegy;

namespace TodoItems.Core;

public class TodoItemService
{
    private readonly ITodoItemsRepository _todosRepository;
    private Dictionary<string, IDuedateStrategy> DuedateSetters = new Dictionary<string, IDuedateStrategy>();
    private IDuedateStrategy earlyDuedateStrategy = new EarlyDuedateStrategy();

    public TodoItemService(ITodoItemsRepository todosRepository)
    {
        _todosRepository = todosRepository;
        DuedateSetters.Add("Early Duedate", new EarlyDuedateStrategy());
        DuedateSetters.Add("Least Count", new LeastCountDuedateStrategy());
    }

    public string GetId()
    {
        return "1";
    }
    public async Task<TodoItemDto> CreateItem(TodoItemDto NewTodoItem, string DuedateStrategy = "Early Duedate")
    {
        DateTime duedate = new DateTime();
        if (NewTodoItem.DueDate != null)
        {
            if (NewTodoItem.CreatedDate > NewTodoItem.DueDate) throw new InvalidOperationException("created date is later than due date.");
            int TodoItemCount = await _todosRepository.GetAllTodoItemsCountInDueDate(NewTodoItem.DueDate);
            if (TodoItemCount > 8) throw new InvalidOperationException("all dates have 8 or more items.");
            duedate = (DateTime)NewTodoItem.DueDate?.Date;
        }
        else
        {
            duedate = (DateTime)await SetDuedate(NewTodoItem, DuedateStrategy);
        }

        NewTodoItem.DueDate = duedate;

        return NewTodoItem;
    }
    public async void UpdateItem(string id, string description)
    {
        var TodoItem = await _todosRepository.FindById(id);
        if (TodoItem == null) return;
        TodoItemDto.Modify(TodoItem, description);
    }
    public TodoItemDto ModifyItem(TodoItemDto curTodoItem)
    {
        if (curTodoItem.TimeStamps.Count > 0)
        {
            DateTime LastDate = curTodoItem.TimeStamps[curTodoItem.TimeStamps.Count - 1];
            if (AreDatesOneDayApart(LastDate, curTodoItem.CreatedDate))
            {
                curTodoItem.TimeStamps.Clear();
            }
            if (curTodoItem.TimeStamps.Count == 3) throw new InvalidOperationException("this item is modified over 3 times");
        }
        curTodoItem.TimeStamps.Add(curTodoItem.CreatedDate);
        UpdateItem(curTodoItem.Id, curTodoItem.Description);

        return curTodoItem;
    }
    public bool AreDatesOneDayApart(DateTime LastDate, DateTime CurDate)
    {
        TimeSpan difference = (LastDate - CurDate).Duration();
        return difference.TotalDays >= 1;
    }

    public async Task<DateTime?> SetDuedate(TodoItemDto todoItemDto,string DuedateStrategy)
    {

        List<TodoItemDto> todoItems = await _todosRepository.GetAllTodoItemsInFiveDays(todoItemDto.CreatedDate);

        var groupedDuedateItems = todoItems
        .GroupBy(item => item.DueDate?.Date)
        .Select(group => new
        {
            DueDate = group.Key,
            Count = group.Count()
        })
        .ToList();

        var futureDates = Enumerable.Range(0, 5)
        .Select(offset => todoItemDto.CreatedDate.Date.AddDays(offset))
        .ToDictionary(date => date, date => 0);

        foreach (var item in groupedDuedateItems)
        {
            if (futureDates.ContainsKey((DateTime)item.DueDate))
            {
                futureDates[(DateTime)item.DueDate] = item.Count; 
            }            
        }

        var duedateSetters = DuedateSetters.GetValueOrDefault(DuedateStrategy, this.earlyDuedateStrategy);
        var duedate = duedateSetters.SetDuedate(futureDates);

        return duedate;

    }
}
