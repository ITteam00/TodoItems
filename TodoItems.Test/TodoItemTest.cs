using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void CompareTimes()
    {
        DateTimeOffset today8AM = DateTimeOffset.Now.Date.AddHours(8);
        DateTimeOffset yesterday8PM = DateTimeOffset.Now.Date.AddDays(-1).AddHours(20);
        var item = new TodoItem();
        Assert.True(item.IsTodady(today8AM));
        Assert.True(!item.IsTodady(yesterday8PM));
    }
}

