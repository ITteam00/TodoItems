using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoItems.Core;

public class TodoItemService
{
    private readonly ItodosRepository _todosRepository;
    public List<DateTime>? TimeStamps;

    public TodoItemService(ItodosRepository todosRepository)
    {
        _todosRepository = todosRepository;
    }

    public string GetId()
    {
        return "1";
    }
    public void UpdateItem(TodoItemDto NewTodoItem)
    {
        if (ModifyItem(NewTodoItem.CreatedDate))
            NewTodoItem.Description = "update";
    }
    public bool CreateItem(TodoItemDto NewTodoItem)
    {
        if (NewTodoItem.CreatedDate > NewTodoItem.DueDate) return false;
        var TodoItems = _todosRepository.FindAllTodoItemsInDueDate();
        if (TodoItems.Count > 8) return false;
        return true;
    }
    public bool ModifyItem(DateTime CreatedDate)
    {
        if (TimeStamps.Count > 0)
        {
            DateTime LastDate = TimeStamps[TimeStamps.Count - 1];

            if (AreDatesOneDayApart(LastDate, CreatedDate))
            {
                TimeStamps.Clear();
                TimeStamps.Add(LastDate);
                return true;
            }
            if (TimeStamps.Count == 3) return false;
        }
        TimeStamps.Add(CreatedDate);

        return true;
    }
    public bool AreDatesOneDayApart(DateTime LastDate, DateTime CurDate)
    {
        TimeSpan difference = (LastDate - CurDate).Duration();
        return difference.TotalDays >= 1;
    }
}
