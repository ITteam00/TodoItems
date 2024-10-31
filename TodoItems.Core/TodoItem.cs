using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoItems.Core;

public class TodoItem
{
    private readonly ItodosRepository _todosRepository;
    public string Id { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public List<DateTime>? TimeStamps;

    public TodoItem(ItodosRepository todosRepository)
    {
        _todosRepository = todosRepository;
    }

    public string GetId()
    {
        return "1";
    }
    public void UpdateItem(TodoItem NewTodoItem)
    {
        if (ModifyItem(NewTodoItem.CreatedDate))
            Description = "update";
    }
    public bool CreateItem(TodoItem NewTodoItem)
    {
        _todosRepository.FindAllTodoItemsInDueDate();
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
