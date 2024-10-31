using MongoDB.Driver;
using System;
using System.Xml.Serialization;
using TodoItems.Core;
using TodoItems.Core.Model;
using TodoItems.Core.Services;
using Moq;
using Xunit;
using Microsoft.VisualBasic;

namespace TodoItems.Test;

public class TodoItemTest
{
    private readonly ToDoItemsService _toDoService;
    private readonly Mock<ITodosRepository> _mockRepository = new Mock<ITodosRepository>();


    [Fact]
    public void should_return_2_when_add_1_1()
    {
        var todoItem = new TodoItem();
        Assert.Equal("1", todoItem.GetId());
    }

    [Fact]
    public void Should_return_modificationCount_when_call_ModificationCount()
    {
        List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
        {
            new DateTimeOffset(2024, 10, 30, 14, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 29, 18, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 17, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 16, 2, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 16, 2, 0, TimeSpan.Zero)
        };
        var todoItem = new ToDoItemsService(_mockRepository.Object);
        Assert.Equal(4, todoItem.ModificationCount(dateTimes));
    }

    [Fact]
    public async Task Should_throw_Exception_when_call_Update()
    {
        List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
        {
            new DateTimeOffset(2024, 10, 30, 14, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 29, 18, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 17, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 16, 2, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 10, 31, 16, 2, 0, TimeSpan.Zero)
        };
        var updatedToDoItem = new ToDoItemDto
        {
            Description = "Test Description",
            isDone = false,
            isFavorite = false,
            CreatedTime = DateTime.UtcNow,
            ModificationDateTimes = dateTimes,
            Id = "1",
        };

        var todoItem = new ToDoItemsService(_mockRepository.Object);
        var str = "";
        try
        {
            await todoItem.CheckCountUpdateAsync(updatedToDoItem.Id, updatedToDoItem);
        }
        catch (TooManyEntriesException e)
        {
            Console.Write(e);
            str = e.Message;
        }


        Assert.Equal(str, "to many");
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
        var updatedToDoItem = new ToDoItemDto
        {
            Description = "Test Description",
            isDone = false,
            isFavorite = false,
            CreatedTime = DateTime.UtcNow,
            ModificationDateTimes = dateTimes,
            Id = "1",
        };

        var todoItem = new ToDoItemsService(_mockRepository.Object);
        await todoItem.UpdateAsync(updatedToDoItem.Id, updatedToDoItem);
        Assert.Equal(updatedToDoItem.ModificationDateTimes.Count, 4);
    }


    [Fact]
    public void GetItemsByDueDate_Should_Return_CorrectItems()
    {
        // Arrange
        var repository = new TodosRepository();
        var dueDate = new DateTimeOffset(2024, 10, 31, 0, 0, 0, TimeSpan.Zero);
        var items = new List<ToDoItemDto>
        {
            new ToDoItemDto { Id = "1", Description = "Task 1", DueDate = dueDate },
            new ToDoItemDto { Id = "2", Description = "Task 2", DueDate = new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero) },
            new ToDoItemDto { Id = "3", Description = "Task 3", DueDate = dueDate }
        };
        TodosRepository.itemsCollection = items;

        // Act
        var result = repository.GetItemsByDueDate(dueDate);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Id == "1");
        Assert.Contains(result, item => item.Id == "3");
    }
}