using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{

    [Fact]
    public void ModifyItem_ShouldModifyDescription_WhenModificationLimitNotReached()
    {
        // Arrange
        var modification = new Modification();
        var todoItem = new TodoItem
        {
            Id = "1",
            Description = "Initial Task",
            ModificationRecord = modification,
            DueDate = DateTimeOffset.Now.AddDays(7),
        };

        // Act
        todoItem.ModifyItem("Updated Task");

        // Assert
        Assert.Equal("Updated Task", todoItem.Description);
        Assert.Single(todoItem.ModificationRecord.ModifiedTimes);
    }

    [Fact]
    public void ModifyItem_ShouldNotModifyDescription_WhenModificationLimitReached()
    {
        // Arrange
        var modification = new Modification
        {
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now
                }
        };
        var todoItem = new TodoItem
        {
            Id = "1",
            Description = "Initial Task",
            ModificationRecord = modification
        };

        // Assert
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => todoItem.ModifyItem("Updated Task"));
        Assert.Equal("Modification limit reached for today.", exception.Message);

        Assert.Equal("Initial Task", todoItem.Description);
    }

    [Fact]
    public void ModifyItem_ShouldModifyDescription_WhenOldModificationsAreRemoved()
    {
        // Arrange
        var modification = new Modification
        {
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now.AddDays(-1), // Old modification
                    DateTimeOffset.Now, // Today's modification
                    DateTimeOffset.Now // Today's modification
                }
        };
        var todoItem = new TodoItem
        {
            Id = "1",
            Description = "Initial Task",
            ModificationRecord = modification
        };

        // Act
        todoItem.ModifyItem("Updated Task");

        // Assert
        Assert.Equal("Updated Task", todoItem.Description);
    }
}

public class ModificationTests
{
    [Fact]
    public void CanModify_ShouldReturnTrue_WhenModificationsAreLessThanLimit()
    {
        // Arrange
        var modification = new Modification
        {
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now
                }
        };

        // Act
        bool result = modification.CanModify();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanModify_ShouldReturnFalse_WhenModificationsReachLimit()
    {
        // Arrange
        var modification = new Modification
        {
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now
                }
        };

        // Act
        bool result = modification.CanModify();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanModify_ShouldRemoveOldModifications()
    {
        // Arrange
        var modification = new Modification
        {
            ModifiedTimes = new List<DateTimeOffset>
                {
                    DateTimeOffset.Now.AddDays(-1), // Old modification
                    DateTimeOffset.Now, // Today's modification
                    DateTimeOffset.Now // Today's modification
                }
        };

        // Act
        bool result = modification.CanModify();

        // Assert
        Assert.True(result);
        Assert.Equal(2, modification.ModifiedTimes.Count); // Only today's modifications should remain
    }

    [Fact]
    public void IsTodady_ShouldReturnTrue_ForTodayDate()
    {
        // Arrange
        var modification = new Modification();
        var today = DateTimeOffset.Now;

        // Act
        bool result = modification.IsTodady(today);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTodady_ShouldReturnFalse_ForNonTodayDate()
    {
        // Arrange
        var modification = new Modification();
        var notToday = DateTimeOffset.Now.AddDays(-1);

        // Act
        bool result = modification.IsTodady(notToday);

        // Assert
        Assert.False(result);
    }
}

