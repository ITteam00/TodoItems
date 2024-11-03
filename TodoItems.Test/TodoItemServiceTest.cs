using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemServiceTest
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
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30),
            TimeStamps = new List<DateTime> {
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30)
        }
    };
        Assert.Equal(false, todoItemService.ModifyItem(todoItemDto));
    }
    [Fact]
    public void Should_return_true_when_modify_item_TimeStamp_is_null()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30),
            TimeStamps = new List<DateTime>()
    };
        Assert.Equal(true, todoItemService.ModifyItem(todoItemDto));
    }
    [Fact]
    public void Should_return_false_when_modify_item_twice_time()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30),
            TimeStamps = new List<DateTime> {
        new DateTime(2024, 10, 30),
        new DateTime(2024, 10, 30),
        }
        };
        var TimeStamps = new List<DateTime>();

        Assert.Equal(true, todoItemService.ModifyItem(todoItemDto));
        Assert.Equal(3, todoItemDto.TimeStamps.Count);

    }

    [Fact]
    public void Should_timestamp_length_equal_1_when_update_item()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            DueDate = new DateTime(2024, 10, 31),
            CreatedDate = new DateTime(2024, 10, 30),
            TimeStamps = new List<DateTime>()
    };
        todoItemService.UpdateItem(todoItemDto.Id, todoItemDto.Description);
        Assert.Equal("new task", todoItemDto.Description);
    }

    [Fact]
    public void Should_return_true_when_dayoffset_bigger_than_1()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 31);
        Assert.Equal(true, todoItemService.AreDatesOneDayApart(date1, date2));
    }

    [Fact]
    public void Should_return_false_when_dayoffset_smaller_than_1()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 30);
        Assert.Equal(false, todoItemService.AreDatesOneDayApart(date1, date2));
    }

    [Fact]
    public async void Should_return_true_when_create_item_duedate_count_less_than_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
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
        bool RealResult = await todoItemService.CreateItem(todoItemDto);
        Assert.Equal(true, RealResult);
    }

    [Fact]
    public async void Should_return_false_when_create_item_duedate_count_over_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
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

        bool RealResult = await todoItemService.CreateItem(todoItemDto);
        Assert.Equal(false, RealResult);
    }

    [Fact]
    public async void Should_return_false_when_create_item_duedate_less_Createdate()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
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

        bool RealResult = await todoItemService.CreateItem(todoItemDto);
        Assert.Equal(false, RealResult);
    }

    [Fact]
    public async void Should_return_early_Duedate_when_create_item_auto_set_in_five_days()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
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

        bool RealResult = await todoItemService.SetEarlyDuedateInFiveDays(todoItemDto);
        Assert.Equal(false, RealResult);
    }
}