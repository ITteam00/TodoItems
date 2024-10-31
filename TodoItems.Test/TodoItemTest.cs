using ToDoItem.Api.Models;
using TodoItems.Core;
using Moq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenDueDateIsBeforeCreatedTimeDate()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var service = new TodoItemProgram(mockRepository.Object);
        var toDoItemModel = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.AddDays(-2).Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 2,
            DueDate = DateTime.Now.AddDays(-5).Date
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(toDoItemModel));
        Assert.Equal("due date cannot be before creation date", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnToDoItemModel_WhenDueDateIsAfterCreatedTimeDate()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var service = new TodoItemProgram(mockRepository.Object);
        var toDoItemModel = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.AddDays(-2).Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 2,
            DueDate = DateTime.Now.AddDays(2).Date
        };

        // Act
        var result = await service.CreateAsync(toDoItemModel);

        // Assert
        Assert.Equal(toDoItemModel, result);
    }



    [Fact]
    public async Task should_add_1_when_edit_if_EditTimes_is_small_and_same_dayAsync()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemModel>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = $"Item {i + 1}",
                Done = false,
                Favorite = false,
                CreatedTimeDate = DateTimeOffset.Now.Date,
                LastModifiedTimeDate = DateTimeOffset.Now.Date,
                EditTimes = 0,
                DueDate = DateTime.Now.AddDays(2).Date

            });
        }
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        var todoItemProgram = new TodoItemProgram(mockRepository.Object);
        var itemNow = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 0,
            DueDate = DateTime.Now.AddDays(2).Date
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
            EditTimes = 1,
            DueDate = DateTime.Now.AddDays(2).Date

        };

        Assert.Equal(expectedItem, itemAfterEdit);
    }

    [Fact]
    public async void should_add_1_when_edit_if_EditTimes_is_small_and_different_day()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemModel>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = $"Item {i + 1}",
                Done = false,
                Favorite = false,
                CreatedTimeDate = DateTimeOffset.Now.Date,
                LastModifiedTimeDate = DateTimeOffset.Now.Date,
                EditTimes = 0,
                DueDate = DateTime.Now.AddDays(2).Date

            });
        }
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        var todoItemProgram = new TodoItemProgram(mockRepository.Object);
        DateTimeOffset oldDateTime = DateTimeOffset.Now.AddDays(-2);
        var itemToBeEdit = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = oldDateTime.Date,
            EditTimes = 2,
            DueDate = DateTime.Now.AddDays(2).Date

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
            EditTimes = 3,
            DueDate = DateTime.Now.AddDays(2).Date

        };

        Assert.Equal(expectedItem, itemAfterEdit);
    }



    [Fact]
    public async Task should_alert_when_edit_if_EditTimes_is_bigAsync()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemModel>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = $"Item {i + 1}",
                Done = false,
                Favorite = false,
                CreatedTimeDate = DateTimeOffset.Now.Date,
                LastModifiedTimeDate = DateTimeOffset.Now.Date,
                EditTimes = 0,
                DueDate = DateTime.Now.AddDays(2).Date

            });
        }
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        var todoItemProgram = new TodoItemProgram(mockRepository.Object);
        var itemNow = new ToDoItemModel
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 3,
            DueDate = DateTime.Now.AddDays(2).Date

        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => todoItemProgram.OnDetectEdit(itemNow));

        Assert.Equal("Too many edits", exception.Message);
    }

    [Fact]
    public async Task should_add_due_date_if_items_less_than_8()
    {
        var mockRepository = new Mock<ITodosRepository>();
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(new List<ToDoItemModel>());

        var todoItemProgram = new TodoItemProgram(mockRepository.Object);
        var item = new ToDoItemModel
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 0,
            DueDate = DateTime.Now.AddDays(2).Date

        };

        bool res = todoItemProgram.AddDueDate(item);

        Assert.True(res);
    }

    [Fact]
    public void should_throw_exception_if_items_more_than_8()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemModel>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = $"Item {i + 1}",
                Done = false,
                Favorite = false,
                CreatedTimeDate = DateTimeOffset.Now.Date,
                LastModifiedTimeDate = DateTimeOffset.Now.Date,
                EditTimes = 0,
                DueDate = DateTime.Now.AddDays(2).Date

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
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 0,
            DueDate = DateTime.Now.AddDays(2).Date

        };

        Assert.Throws<InvalidOperationException>(() => todoItemProgram.AddDueDate(item));
    }



}