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
        var service = new TodoItemService(mockRepository.Object);
        var toDoItemModel = new ToDoItemDto
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
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenMoreThan8ItemsExistForToday()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var service = new TodoItemService(mockRepository.Object);
        var toDoItemModel = new ToDoItemDto
        {
            Id = "1",
            Description = "Item 1",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTimeOffset.Now.Date,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = 2,
            DueDate = DateTime.Now.AddDays(2).Date
        };

        var todayItems = new List<ToDoItemDto>();
        for (int i = 0; i < 8; i++)
        {
            todayItems.Add(new ToDoItemDto
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

        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(todayItems);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(toDoItemModel));
        Assert.Equal("Cannot add more than 8 ToDo items for today.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnToDoItemModel_WhenDueDateIsAfterCreatedTimeDate_AndItemsAreLessThan8()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var service = new TodoItemService(mockRepository.Object);
        var toDoItemModel = new ToDoItemDto
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

        var todayItems = new List<ToDoItemDto>();
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(todayItems);

        // Act
        var result = await service.CreateAsync(toDoItemModel);

        // Assert
        Assert.Equal(toDoItemModel, result);
    }




    [Fact]
    public async Task CreateAsync_ShouldReturnToDoItemModel_WhenDueDateIsAfterCreatedTimeDate()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var service = new TodoItemService(mockRepository.Object);
        var toDoItemModel = new ToDoItemDto
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

        var todayItems = new List<ToDoItemDto>(); // Ensure this list is not null
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(todayItems);

        // Act
        var result = await service.CreateAsync(toDoItemModel);

        // Assert
        Assert.Equal(toDoItemModel, result);
    }




    [Fact]
    public async Task should_add_1_when_edit_if_EditTimes_is_small_and_same_dayAsync()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemDto>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemDto
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

        var todoItemProgram = new TodoItemService(mockRepository.Object);
        var itemNow = new ToDoItemDto
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
        ToDoItemDto itemAfterEdit = await todoItemProgram.ModifyItem(itemNow);

        var expectedItem = new ToDoItemDto
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
        var items = new List<ToDoItemDto>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemDto
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

        var todoItemProgram = new TodoItemService(mockRepository.Object);
        DateTimeOffset oldDateTime = DateTimeOffset.Now.AddDays(-2);
        var itemToBeEdit = new ToDoItemDto
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
        ToDoItemDto itemAfterEdit = await todoItemProgram.ModifyItem(itemToBeEdit);


        var expectedItem = new ToDoItemDto
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
        var items = new List<ToDoItemDto>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemDto
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

        var todoItemProgram = new TodoItemService(mockRepository.Object);
        var itemNow = new ToDoItemDto
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

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => todoItemProgram.ModifyItem(itemNow));

        Assert.Equal("Too many edits", exception.Message);
    }




}