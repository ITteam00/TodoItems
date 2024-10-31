using ToDoItem.Api.Models;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public async Task should_add_1_when_edit_if_EditTimes_is_small_and_same_dayAsync()
    {
        var todoItemProgram = new TodoItemProgram();
        var itemNow = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 0
        };
        ToDoItemModel itemAfterEdit = await todoItemProgram.OnDetectEdit(itemNow);

        var expectedItem = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 1
        };

        Assert.Equal(expectedItem, itemAfterEdit);
    }

    [Fact]
    public async void should_add_1_when_edit_if_EditTimes_is_small_and_different_day()
    {
        var todoItemProgram = new TodoItemProgram();
        DateTimeOffset oldDateTime = DateTimeOffset.Now.AddDays(-2);
        var itemToBeEdit = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = oldDateTime.Date,
            EditTimes = 2
        };
        ToDoItemModel itemAfterEdit = await todoItemProgram.OnDetectEdit(itemToBeEdit);


        var expectedItem = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 3
        };

        Assert.Equal(expectedItem, itemAfterEdit);
    }



    [Fact]
    public async Task should_alert_when_edit_if_EditTimes_is_bigAsync()
    {
        var todoItemProgram = new TodoItemProgram();
        var itemNow = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 3
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => todoItemProgram.OnDetectEdit(itemNow));

        // ??????
        Assert.Equal("Too many edits", exception.Message);
    }



}