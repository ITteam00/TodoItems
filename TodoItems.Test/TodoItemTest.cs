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

        Assert.Equal("Too many edits", exception.Message);
    }

    [Fact]
    public async Task should_add_due_date_if_items_less_than_8()
    {
        var mockRepository = new Mock<TodosRepository>();
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(new List<ToDoItemModel>());

        var todoItemProgram = new TodoItemProgram(mockRepository.Object);
        var item = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = DateTimeOffset.Now,
            EditTimes = 0
        };

        var dueDate = DateTimeOffset.Now.AddDays(1);
        todoItemProgram.AddDueDate(item, dueDate);

        Assert.Equal(dueDate, item.DueDate);
    }

    [Fact]
    public void should_throw_exception_if_items_more_than_8()
    {
        var mockRepository = new Mock<TodosRepository>();
        var items = new List<ToDoItemModel>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = $"Item {i + 1}",
                Done = false,
                Favorite = false,
                CreatedTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EditTimes = 0
            });
        }
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        var todoItemProgram = new TodoItemProgram(mockRepository.Object);
        var item = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "New Item",
            Done = false,
            Favorite = false,
            CreatedTime = DateTimeOffset.Now,
            LastModifiedTime = DateTimeOffset.Now,
            EditTimes = 0
        };

        var dueDate = DateTimeOffset.Now.AddDays(1);

        Assert.Throws<InvalidOperationException>(() => todoItemProgram.AddDueDate(item, dueDate));
    }



}