﻿using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        var futureDates = Enumerable.Range(0, 5)
        .Select(offset => todoItemDto.CreatedDate.Date.AddDays(offset))
        .ToList();

        List<TodoItemDto> todoItems = await _todosRepository.GetAllTodoItemsInFiveDays(todoItemDto.CreatedDate);

        var groupedDuedateItems = todoItems
        .GroupBy(item => item.DueDate.Date)
        .Select(group => new
        {
            DueDate = group.Key,
            Count = group.Count()
        })
        .ToList();

        var dueDateCounts = futureDates.ToDictionary(date => date, date => 0);

        foreach (var item in groupedDuedateItems)
        {
            if (dueDateCounts.ContainsKey(item.DueDate))
            {
                dueDateCounts[item.DueDate] = item.Count; 
            }
        }

        var earliestDueDate = dueDateCounts
            .Where(kvp => kvp.Value < 8) 
            .Select(kvp => kvp.Key)
            .OrderBy(date => date)
            .FirstOrDefault();

        if (earliestDueDate == default(DateTime))
        {
            throw new InvalidOperationException("No valid due date found, all dates have 8 or more items.");
        }

        return earliestDueDate;

    }
}
