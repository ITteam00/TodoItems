﻿using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoItems.Core;

public class TodoItemService
{
    private readonly ITodoItemsRepository _todosRepository;

    public TodoItemService(ITodoItemsRepository todosRepository)
    {
        _todosRepository = todosRepository;
    }

    public string GetId()
    {
        return "1";
    }
    public async Task<bool> CreateItem(TodoItemDto NewTodoItem)
    {
        if (NewTodoItem.CreatedDate > NewTodoItem.DueDate) return false;
        int TodoItemCount = await _todosRepository.GetAllTodoItemsCountInDueDate(NewTodoItem.DueDate);
        if (TodoItemCount > 8) return false;
        return true;
    }
    public async void UpdateItem(string id, string description)
    {
        var TodoItem = await _todosRepository.FindById(id);
        if (TodoItem == null) return;
        TodoItem.Description = description;
    }
    public bool ModifyItem(TodoItemDto curTodoItem)
    {
        if (curTodoItem.TimeStamps.Count > 0)
        {

            DateTime LastDate = curTodoItem.TimeStamps[curTodoItem.TimeStamps.Count - 1];

            if (AreDatesOneDayApart(LastDate, curTodoItem.CreatedDate))
            {
                curTodoItem.TimeStamps.Clear();
                curTodoItem.TimeStamps.Add(LastDate);
                return true;
            }
            if (curTodoItem.TimeStamps.Count == 3) return false;
        }
        curTodoItem.TimeStamps.Add(curTodoItem.CreatedDate);
        UpdateItem(curTodoItem.Id, curTodoItem.Description);

        return true;
    }
    public bool AreDatesOneDayApart(DateTime LastDate, DateTime CurDate)
    {
        TimeSpan difference = (LastDate - CurDate).Duration();
        return difference.TotalDays >= 1;
    }

    public async Task<DateTime?> SetEarlyDuedateInFiveDays(TodoItemDto todoItemDto)
    {
        List<TodoItemDto> todoItems = await _todosRepository.GetAllTodoItemsInFiveDays(todoItemDto.CreatedDate);
        if (todoItems.Count == 0) return todoItemDto.CreatedDate;
        var groupedDuedateItems = todoItems
        .GroupBy(item => item.DueDate.Date)
        .Select(group => new
        {
            DueDate = group.Key,
            Count = group.Count()
        })
        .ToList();

        var validDueDates = groupedDuedateItems
            .Where(group => group.Count < 8)
            .Select(group => group.DueDate)
            .ToList();
        if (validDueDates.Any())
        {
            return validDueDates.Min();
        }

        throw new NotImplementedException();
    }
}
