using System.Runtime.CompilerServices;
using TodoItems.Core.services;

namespace TodoItems.Core;

public class TodoItem
{
    public string? Id { get; set; }
    public string Description { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime[]? ModifyTime { get; set; }
    public Boolean IsComplete { get; set; }
    public Boolean IsFavorite { get; set; }

    public string GetId()
    {
        return this.Id;
    }

    public TodoItem ModifyTodoItem(TodoItem oldItem,TodoItem newItem)
    {
        if (oldItem.ModifyTime[oldItem.ModifyTime.Length-1].Date!=DateTime.Now.Date)
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
