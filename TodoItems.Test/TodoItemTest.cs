using System;
using System.Xml.Serialization;
using TodoItems.Core;
using TodoItems.Core.Model;
using TodoItems.Core.Services;

namespace TodoItems.Test;

public class TodoItemTest
{
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
        var todoItem = new ToDoItemsService();
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

        var todoItem = new ToDoItemsService();
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
}