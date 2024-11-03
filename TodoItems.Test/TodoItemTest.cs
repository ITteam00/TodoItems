using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void should_return_2_when_add_1_1()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        Assert.Equal("1", todoItem.GetId());
    }
    [Fact]
    public void Should_return_false_when_modify_item_third_time()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30)
        };
        todoItem.TimeStamps = new List<DateTime> {
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30)
        };
        Assert.Equal(false, todoItem.ModifyItem(todoItemDto.CreatedDate));
    }
    [Fact]
    public void Should_return_true_when_modify_item_TimeStamp_is_null()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30)
        };

        todoItem.TimeStamps = new List<DateTime>();
        Assert.Equal(true, todoItem.ModifyItem(todoItemDto.CreatedDate));
    }
    [Fact]
    public void Should_return_false_when_modify_item_twice_time()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30)
        };
        todoItem.TimeStamps = new List<DateTime> {
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30),
        };
        var TimeStamps = new List<DateTime>();

        Assert.Equal(true, todoItem.ModifyItem(todoItemDto.CreatedDate));
        Assert.Equal(3, todoItem.TimeStamps.Count);

    }

    [Fact]
    public void Should_timestamp_length_equal_1_when_update_item()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30)
        };
        todoItem.TimeStamps = new List<DateTime>();
        todoItem.UpdateItem(todoItemDto);
        Assert.Equal("update", todoItemDto.Description);
    }

    [Fact]
    public void Should_return_true_when_dayoffset_bigger_than_1()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 31);
        Assert.Equal(true, todoItem.AreDatesOneDayApart(date1, date2));
    }

    [Fact]
    public void Should_return_false_when_dayoffset_smaller_than_1()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 30);
        Assert.Equal(false, todoItem.AreDatesOneDayApart(date1, date2));
    }

    [Fact]
    public async void Should_return_true_when_create_item_duedate_count_less_than_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30)
        };
        mockRepository.Setup(repo => repo.GetAllTodoItemsCountInDueDate(It.IsAny<DateTime>())).ReturnsAsync(6);
        bool RealResult = await todoItem.CreateItem(todoItemDto);
        Assert.Equal(true, RealResult);
    }

    [Fact]
    public async void Should_return_false_when_create_item_duedate_count_over_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30)
        };
        mockRepository.Setup(repo => repo.GetAllTodoItemsCountInDueDate(It.IsAny<DateTime>())).ReturnsAsync(10);

        bool RealResult = await todoItem.CreateItem(todoItemDto);
        Assert.Equal(false, RealResult);
    }

    [Fact]
    public async void Should_return_false_when_create_item_duedate_less_Createdate()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItem = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 29),
            CreatedDate = new DateTime(2024, 10, 30)
        };
        mockRepository.Setup(repo => repo.GetAllTodoItemsCountInDueDate(It.IsAny<DateTime>())).ReturnsAsync(6);

        bool RealResult = await todoItem.CreateItem(todoItemDto);
        Assert.Equal(false, RealResult);
    }
}