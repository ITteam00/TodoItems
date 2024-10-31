using System;

namespace TodoItems.Core;

public class TodoItem
{
    public string GetId()
    {
        return "1";
    }

    public int ModificationCount(List<DateTimeOffset> modificationDateTimes)
    {
        var count = modificationDateTimes.Count(d => d.Date == DateTimeOffset.UtcNow.Date); 
        return count;
    }
}
