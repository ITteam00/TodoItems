using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void should_update_when_time_less_than_three()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime= [],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [],
        };
        try
        {
            var updatedItem = oldItem.ModifyTodoItem(oldItem, updateItem);
        }
        catch (Exception ex) { }
        Assert.Equal("should update description",updateItem.Description);
    }

    [Fact]
    public void should_throw_exception_when_time_more_than_three()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 10, 31, 00, 00, 01), new DateTime(2024, 10, 31, 00, 01, 01), new DateTime(2024, 10, 31, 01, 00, 01)],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        try
        {
            oldItem.ModifyTodoItem(oldItem, updateItem);
        }
        catch (Exception ex) {
            Assert.Equal(ex.Message, "No modify time");
        }
        Assert.Equal("old item description",oldItem.Description);
    }

    [Fact]
    public void should_update_todoItem_when_across_day()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 10, 30, 00, 00, 01), new DateTime(2024, 10, 30, 00, 01, 01), new DateTime(2024, 10, 30, 01, 00, 01)],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        var updatedItem = oldItem.ModifyTodoItem(oldItem, updateItem);

        Assert.Equal("should update description", updatedItem.Description);
        Assert.Equal(1, updatedItem.ModifyTime.Length);
    }
}