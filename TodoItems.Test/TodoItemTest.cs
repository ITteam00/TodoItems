using MongoDB.Driver;
using System;
using System.Xml.Serialization;
using TodoItems.Core;
using TodoItems.Core.Model;
using TodoItems.Core.Services;
using Moq;
using Xunit;
using Microsoft.VisualBasic;
using TodoItems.Core.Validator;

namespace TodoItems.Test;

public class TodoItemTest
{
    private readonly ToDoItemsService _toDoService;
    private readonly TodoItemsRepository _todoItemsRepository;
    private readonly TodoItemValidator _todoItemValidator;
    private readonly Mock<ITodosRepository> _mockRepository;

    public TodoItemTest()
    {
        _mockRepository = new Mock<ITodosRepository>();
        _toDoService = new ToDoItemsService(_mockRepository.Object);
        _todoItemValidator = new TodoItemValidator();
    }


    [Fact]
    public void should_return_2_when_add_1_1()
    {
        var todoItem = new TodoItem();
        Assert.Equal("1", todoItem.GetId());
    }

    [Fact]
    public async Task Should_throw_Exception_when_call_Update()
    {
        List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
        {
            new DateTimeOffset(2024, 10, 30, 14, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 11, 1, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 29, 18, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 11, 1, 17, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 11, 1, 16, 2, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 11, 1, 16, 2, 0, TimeSpan.Zero)
        };
        var updatedToDoItem = new TodoItemDTO
        {
            Description = "Test Description",
            IsDone = false,
            IsFavorite = false,
            CreatedTime = DateTime.UtcNow,
            ModificationDateTimes = dateTimes,
            Id = "1",
        };
        var exception =
            await Assert.ThrowsAsync<TooManyEntriesException>(() =>
                _toDoService.UpdateAsync(updatedToDoItem.Id, updatedToDoItem));
        Assert.Equal(exception.Message, "to many");
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateItemInCollection()
    {
        // Arrange
        var id = "test-id";
        List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
        {
            new DateTimeOffset(2024, 10, 30, 14, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 29, 18, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 17, 0, 0, TimeSpan.Zero),
        };
        var updatedToDoItem = new TodoItemDTO
        {
            Description = "Test Description",
            IsDone = false,
            IsFavorite = false,
            CreatedTime = DateTime.UtcNow,
            ModificationDateTimes = dateTimes,
            Id = "1",
        };

        _mockRepository.Setup(r => r.UpdateAsync(updatedToDoItem.Id, updatedToDoItem))
            .Returns(Task.CompletedTask);

        await _toDoService.UpdateAsync(updatedToDoItem.Id, updatedToDoItem);

        Assert.Equal(4, updatedToDoItem.ModificationDateTimes.Count);
    }

    [Fact]
    public void IsTodady_ShouldReturnTrue_WhenDateIsToday()
    {
        var service = new ToDoItemsService(_mockRepository.Object);
        var today = DateTimeOffset.Now;
        var result = service.IsTodady(today);
        Assert.True(result);
    }

    [Fact]
    public void IsTodady_ShouldReturnFalse_WhenDateIsNotToday()
    {
        var service = new ToDoItemsService(_mockRepository.Object);
        var notToday = DateTimeOffset.Now.AddDays(-1);
        var result = service.IsTodady(notToday);
        Assert.False(result);
    }

    [Fact]
    public void CanModify_ShouldReturnFalse_WhenModificationLimitExceeded()
    {
        // Arrange
        var service = new ToDoItemsService(_mockRepository.Object);
        var today = DateTimeOffset.Now.Date;
        var updatedToDoItem = new TodoItemDTO
        {
            Id = "test-id",
            ModificationDateTimes = new List<DateTimeOffset>
            {
                today.AddHours(1),
                today.AddHours(2),
                today.AddHours(3),
                today.AddHours(1),
                today.AddHours(1),
                today.AddHours(2),
                today.AddHours(3),
                today.AddHours(1),
                today.AddHours(2),
                today.AddHours(3),
            }
        };

        // Act
        var result = service.CanModify(updatedToDoItem);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanModify_ShouldReturnTrue_WhenModificationLimitNotExceeded()
    {
        // Arrange
        var service = new ToDoItemsService(_mockRepository.Object);
        var today = DateTimeOffset.Now.Date;
        var updatedToDoItem = new TodoItemDTO
        {
            Id = "test-id",
            ModificationDateTimes = new List<DateTimeOffset>
            {
                today.AddHours(1),
                today.AddHours(2)
            }
        };

        // Act
        var result = service.CanModify(updatedToDoItem);

        // Assert
        Assert.True(result);
    }
}