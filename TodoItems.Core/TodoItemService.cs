using System.Runtime.CompilerServices;
using TodoItems.Core;


namespace TodoItems.Core;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoItemsRepository _todoRepository;

    public TodoItemService(ITodoItemsRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoItem> CreateTodoItem(TodoItem item, string? type="")
    {
        if (item.DueTime == null) { 

            

        }
        else
        {
            List<TodoItem> todoItems = await _todoRepository.getAllItemsCountInToday(item.DueTime.Date);
            if (todoItems.Count > 8)
            {
                throw new TooManyTodoItemInDueDateException("A maximum of eight todoitems can be completed per day");
            }
            else
            {
                await _todoRepository.SaveAsync(item);
                return item;
            }
        }
    }


    //public TodoItem CreateTodoItem(TodoItem item)
    //{
    //    item.Id = Guid.NewGuid().ToString();
    //    if (item.DueTime < item.CreateTime)
    //    {
    //        throw new Exception("Due time should later than create time");
    //    }
    //    var todayAllItem = _todoRepository.getAllItemsCountInToday(item.DueTime);
    //    if (todayAllItem.Count > 8)
    //    {
    //        throw new Exception("A maximum of eight todoitems can be completed per day");
    //    }
    //    return item;
    //}

    public TodoItem ModifyTodoItem(TodoItem oldItem, TodoItem newItem)
    {
        if (oldItem.ModifyTime[oldItem.ModifyTime.Length - 1].Date != DateTime.Now.Date)
        {
            oldItem.ModifyTime = [DateTime.Now];
            ModifyItemDescription(oldItem, newItem);
        }
        else
        {
            if (oldItem.ModifyTime.Length >= 3)
            {
                throw new Exception("No modify time");
            }
            else
            {
                ModifyItemDescription(oldItem, newItem);
            }
        }
        return oldItem;
    }
    private static void ModifyItemDescription(TodoItem oldItem, TodoItem newItem)
    {
        oldItem.Description = newItem.Description;
    }
}
