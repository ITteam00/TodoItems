using System.Runtime.CompilerServices;
using TodoItems.Core.services;

namespace TodoItems.Core;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoRepository _todoRepository;

    public TodoItemService(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public TodoItemDto CreateTodoItem(TodoItemDto item)
    {
        item.Id = Guid.NewGuid().ToString();
        if (item.DueTime < item.CreateTime)
        {
            throw new Exception("Due time should later than create time");
        }
        var todayAllItem = _todoRepository.getAllItemsCountInToday(item.DueTime);
        if (todayAllItem.Count > 8)
        {
            throw new Exception("A maximum of eight todoitems can be completed per day");
        }
        return item;
    }

    public TodoItemDto ModifyTodoItem(TodoItemDto oldItem, TodoItemDto newItem)
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
    private static void ModifyItemDescription(TodoItemDto oldItem, TodoItemDto newItem)
    {
        oldItem.Description = newItem.Description;
    }
}
