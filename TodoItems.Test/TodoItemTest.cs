using ToDoItem.Api.Models;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void should_add_1_when_edit_if_EditTimes_is_small_and_same_day()
    {
        var todoItemProgram = new TodoItemProgram();
        var itemNow = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = DateTimeOffset.Now,
            EditTimes = 0
        };
        ToDoItemModel itemAfterEdit = todoItemProgram.OnDetectEdit(itemNow);

        var expectedItem = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = DateTimeOffset.Now,
            EditTimes = 1
        };

        Assert.Equal(expectedItem, itemAfterEdit);
    }

    [Fact]
    public void should_add_1_when_edit_if_EditTimes_is_small_and_different_day()
    {
        var todoItemProgram = new TodoItemProgram();
        DateTimeOffset oldDateTime = DateTimeOffset.Now.AddDays(-2);
        var itemNow = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = oldDateTime.Date,
            EditTimes = 2
        };
        ToDoItemModel itemAfterEdit = todoItemProgram.OnDetectEdit(itemNow);


        var expectedItem = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = DateTimeOffset.Now,
            EditTimes = 1
        };

        Assert.Equal(expectedItem, itemAfterEdit);
    }



    [Fact]
    public async Task should_alert_when_edit_if_EditTimes_is_bigAsync()
    {
        var todoItemProgram = new TodoItemProgram();
        var itemNow = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = DateTimeOffset.Now,
            EditTimes = 3
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => todoItemProgram.OnDetectEdit(itemNow));

        // ??????
        Assert.Equal("Your edit time run out", exception.Message);
    }



}