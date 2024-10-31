using ToDoItem.Api.Models;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void should_add_1_when_edit_if_EditTimes_is_small()
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
        var itemAfterEdit = todoItemProgram.OnDetectEdit(itemNow);
        //var itemAfterEdit = todoItemProgram.AddEditTimes(itemNow);


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
    public void should_alert_when_edit_if_EditTimes_is_big()
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
        string response = todoItemProgram.OnDetectEdit(itemNow);
        //string response = todoItemProgram.AlertMessage(itemNow);


        var expectedItem = "Your edit time run out";

        Assert.Equal(expectedItem, response);
    }



}