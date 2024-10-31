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
    [Fact]
    public void ModifyItem_ShouldModifyDescription_WhenModificationLimitNotReached()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = "1",
            Description = "Initial Task",
            ModifiedTimes = new List<DateTimeOffset>()
        };

        // Act
        bool result = todoItem.ModifyItem("Updated Task");

        // Assert
        Assert.True(result);
        Assert.Equal("Updated Task", todoItem.Description);
    }

    [Fact]
    public void ModifyItem_ShouldNotModifyDescription_WhenModificationLimitReached()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = "1",
            Description = "Initial Task",
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now
                }
        };

        // Act
        bool result = todoItem.ModifyItem("Updated Task");

        // Assert
        Assert.False(result);
        Assert.Equal("Initial Task", todoItem.Description);
    }

    [Fact]
    public void ModifyItem_ShouldModifyDescription_WhenOldModificationsAreRemoved()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = "1",
            Description = "Initial Task",
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now.AddDays(-1), // Old modification
                    DateTimeOffset.Now, // Today's modification
                    DateTimeOffset.Now // Today's modification
                }
        };

        // Act
        bool result = todoItem.ModifyItem("Updated Task");

        // Assert
        Assert.True(result);
        Assert.Equal("Updated Task", todoItem.Description);
    }
}

