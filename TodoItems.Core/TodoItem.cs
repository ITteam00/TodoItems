﻿namespace TodoItems.Core;

public class TodoItem
{
    public string Id { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public List<DateTime> TimeStamps;

    public string GetId()
    {
        return "1";
    }

    public bool ModifyItem(List<DateTime> TimeStamps, string id) 
    {
        return true;
    }
}