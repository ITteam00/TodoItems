using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void should_return_2_when_add_1_1()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        Assert.Equal("1", todoItem.GetId());
    }
    [Fact]
    public void Should_return_false_when_modify_item_third_time()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        todoItem.Id = "1";
        todoItem.CreatedDate = new DateTime(2024, 10, 30);
        todoItem.TimeStamps = new List<DateTime> {
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30)
        };
        Assert.Equal(false, todoItem.ModifyItem(todoItem.CreatedDate));
    }
    [Fact]
    public void Should_return_true_when_modify_item_TimeStamp_is_null()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        todoItem.Id = "1";
        todoItem.CreatedDate = DateTime.Now;

        todoItem.TimeStamps = new List<DateTime>();
        Assert.Equal(true, todoItem.ModifyItem(todoItem.CreatedDate));
    }
    [Fact]
    public void Should_return_false_when_modify_item_twice_time()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        todoItem.Id = "1";
        todoItem.CreatedDate = new DateTime(2024, 10, 30);
        todoItem.TimeStamps = new List<DateTime> {
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30),
        };
        var TimeStamps = new List<DateTime>();

        Assert.Equal(true, todoItem.ModifyItem(todoItem.CreatedDate));
        Assert.Equal(3, todoItem.TimeStamps.Count);

    }

    [Fact]
    public void Should_timestamp_length_equal_1_when_update_item()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        todoItem.Id = "1";
        todoItem.CreatedDate = DateTime.Now;
        todoItem.TimeStamps = new List<DateTime>();
        todoItem.UpdateItem(todoItem);
        Assert.Equal("update", todoItem.Description);
    }

    [Fact]
    public void Should_return_true_when_dayoffset_bigger_than_1()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 31);
        Assert.Equal(true, todoItem.AreDatesOneDayApart(date1, date2));
    }

    [Fact]
    public void Should_return_false_when_dayoffset_smaller_than_1()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 30);
        Assert.Equal(false, todoItem.AreDatesOneDayApart(date1, date2));
    }

    [Fact]
    public void Should_return_true_when_create_item_duedate_count_less_than_8()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        var TodoItems = new List<TodoItem> { };
        for (int i = 0; i < 6; i++)
        {
            TodoItems.Add(new TodoItem(mockRepository.Object));
        }
        mockRepository.Setup(repo => repo.FindAllTodoItemsInDueDate()).Returns(TodoItems);

        Assert.Equal(true, todoItem.CreateItem(todoItem));
    }

    [Fact]
    public void Should_return_true_when_create_item_duedate_count_over_8()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        var TodoItems = new List<TodoItem> {};
        for (int i = 0; i < 10; i++)
        {
            TodoItems.Add(new TodoItem(mockRepository.Object));
        }
        mockRepository.Setup(repo => repo.FindAllTodoItemsInDueDate()).Returns(TodoItems);

        Assert.Equal(false, todoItem.CreateItem(todoItem));
    }

    [Fact]
    public void Should_return_false_when_create_item_duedate_less_Createdate()
    {
        var mockRepository = new Mock<ItodosRepository>();
        var todoItem = new TodoItem(mockRepository.Object);
        var TodoItems = new List<TodoItem> { };
        for (int i = 0; i < 5; i++)
        {
            TodoItems.Add(new TodoItem(mockRepository.Object));
        }
        todoItem.CreatedDate = new DateTime(2024, 10, 30);
        todoItem.DueDate = new DateTime(2024, 10, 15);
        mockRepository.Setup(repo => repo.FindAllTodoItemsInDueDate()).Returns(TodoItems);

        Assert.Equal(true, todoItem.CreateItem(todoItem));
    }
}