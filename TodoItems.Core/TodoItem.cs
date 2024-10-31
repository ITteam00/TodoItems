using System;

namespace TodoItems.Core;


public class Program
{
    public static void Main(string[] args)
    {
        Console.Write("999");
        DateTimeOffset todayMidnight = DateTimeOffset.Now.Date;

        // 输出今天凌晨0点的时间
        Console.WriteLine("今天凌晨0点的时间是: " + todayMidnight.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}


public class TodoItem
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset[] ModifiedTimes { get; set; }

    public bool ModifyItem(string newTaskDescription)
    {

        Description = newTaskDescription;
        return true;
    }

    public bool IsTodady(DateTimeOffset dateTime)
    {
        var toady = DateTimeOffset.Now.Date;
        return dateTime.Date == toady;
    }

}

