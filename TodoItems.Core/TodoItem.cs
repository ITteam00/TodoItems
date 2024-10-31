﻿using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoItems.Core;

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
    public void UpdateItem(string id, DateTime CreatedDate)
    {
        TimeStamps.Add(CreatedDate);
    }
    public bool ModifyItem(List<DateTime> TimeStamps, DateTime CreatedDate,string id) 
    {
        return true;
    }
    public bool AreDatesOneDayApart(DateTime LastDate, DateTime CurDate) 
    {
        TimeSpan difference = (LastDate - CurDate).Duration(); 
        return difference.TotalDays >= 1; 
    }
}
