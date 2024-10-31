using static System.Runtime.InteropServices.JavaScript.JSType;

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
    public bool ModifyItem(DateTime CreatedDate) 
    {
        if (TimeStamps == null) return false;
        DateTime LastDate = TimeStamps[TimeStamps.Count - 1];

        if (AreDatesOneDayApart(LastDate, CreatedDate))
        {
            TimeStamps.Clear();
            TimeStamps.Add(LastDate);
            return true;
        }

        if(TimeStamps.Count == 3) return false;

        TimeStamps.Add(LastDate);

        return true;
    }
    public bool AreDatesOneDayApart(DateTime LastDate, DateTime CurDate) 
    {
        TimeSpan difference = (LastDate - CurDate).Duration(); 
        return difference.TotalDays >= 1; 
    }
}
