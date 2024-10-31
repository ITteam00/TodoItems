namespace TodoItems.Core;

public class TodoItem
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public List<DateTimeOffset> ModifiedTimes { get; set; }

    public const int MaximumModificationNumber = 3;

    public bool ModifyItem(string newTaskDescription)
    {
        ModifiedTimes = ModifiedTimes.Where(t => IsTodady(t)).ToList();
        if (ModifiedTimes.Count >= MaximumModificationNumber)
        {
            return false;
        }
        else
        {
            Description = newTaskDescription;
            return true;

        }
    }

    public bool IsTodady(DateTimeOffset dateTime)
    {
        var toady = DateTimeOffset.Now.Date;
        return dateTime.Date == toady;
    }

}

