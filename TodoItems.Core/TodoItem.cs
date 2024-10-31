﻿using System.Runtime.CompilerServices;
using TodoItems.Core.services;

namespace TodoItems.Core;

public class TodoItem
{
    private readonly ITodoRepository _todoRepository;
    public DateTime? CreateTime { get; set; }
    public string Description { get; set; }
    public DateTime DueTime { get; set; }
    public string? Id { get; set; }
    public Boolean IsComplete { get; set; }
    public Boolean IsFavorite { get; set; }
    public DateTime[]? ModifyTime { get; set; }

    public TodoItem(ITodoRepository todoRepository) {
        _todoRepository = todoRepository;}

    public string GetId()
    {
        return this.Id;
    }

    public TodoItem CreateTodoItem(TodoItem item)
    {
        item.Id=Guid.NewGuid().ToString();
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
