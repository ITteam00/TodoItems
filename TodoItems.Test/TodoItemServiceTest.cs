using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
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
    public async void Should_return_todoItem_when_create_item_duedate_count_less_than_8()
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
        var todoItem = await todoItemService.CreateItem(todoItemDto);
        Assert.Equal(todoItemDto, todoItem);
    }

    [Fact]
    public async Task Should_return_false_when_create_item_duedate_count_over_8()
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

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => todoItemService.CreateItem(todoItemDto));
        Assert.Equal("all dates have 8 or more items.", exception.Message);
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

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => todoItemService.CreateItem(todoItemDto));
        Assert.Equal("created date is later than due date.", exception.Message);
    }

    [Fact]
    public async void Should_return_early_Duedate_when_create_item_auto_set_in_five_days_query_none()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            CreatedDate = new DateTime(2024, 10, 30)
        };
        List<TodoItemDto> todoItems = new List<TodoItemDto>();
        mockRepository.Setup(repo => repo.GetAllTodoItemsInFiveDays(It.IsAny<DateTime>())).ReturnsAsync(todoItems);

        DateTime RealDueDate = (DateTime)await todoItemService.SetDuedate(todoItemDto, "Early Duedate");
        Assert.Equal(todoItemDto.CreatedDate, RealDueDate.Date);
    }

    [Fact]
    public async void Should_return_second_early_Duedate_when_create_item_auto_set_in_five_days_query_less_than_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            CreatedDate = new DateTime(2024, 10, 10)
        };
        List<TodoItemDto> todoItems = new List<TodoItemDto>();
        for (int i = 0; i < 8; i++)
        {
            var todoItem = new TodoItemDto
            {
                Id = "1" + i.ToString(),
                Description = "new task",
                IsDone = true,
                IsFavorite = true,
                DueDate = new DateTime(2024, 10, 10)
            };
            todoItems.Add(todoItem);
        }
        mockRepository.Setup(repo => repo.GetAllTodoItemsInFiveDays(It.IsAny<DateTime>())).ReturnsAsync(todoItems);

        DateTime RealDueDate = (DateTime)await todoItemService.SetDuedate(todoItemDto, "Early Duedate");
        DateTime ExpectedDuedate = new DateTime(2024, 10, 11);
        Assert.Equal(ExpectedDuedate.Date, RealDueDate.Date);
    }

    [Fact]
    public async void Should_return_default_Duedate_when_create_item_set_default_duedate()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            CreatedDate = new DateTime(2024, 10, 10),
            DueDate = new DateTime(2024, 11, 11)
        };
        List<TodoItemDto> todoItems = new List<TodoItemDto>();
        for (int i = 0; i < 1; i++)
        {
            var todoItem = new TodoItemDto
            {
                Id = "1" + i.ToString(),
                Description = "new task",
                IsDone = true,
                IsFavorite = true,
                DueDate = new DateTime(2024, 10, 10)
            };
            todoItems.Add(todoItem);
        }
        mockRepository.Setup(repo => repo.GetAllTodoItemsInFiveDays(It.IsAny<DateTime>())).ReturnsAsync(todoItems);

        var RealTodoItem = await todoItemService.CreateItem(todoItemDto, "Early Duedate");
        Assert.Equal(todoItemDto.DueDate?.Date, RealTodoItem.DueDate?.Date);
    }

    [Fact]
    public async void Should_return_least_count_Duedate_when_create_item_auto_set_in_five_days_query_less_than_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            CreatedDate = new DateTime(2024, 10, 30)
        };
        List<TodoItemDto> todoItems = new List<TodoItemDto>();
        mockRepository.Setup(repo => repo.GetAllTodoItemsInFiveDays(It.IsAny<DateTime>())).ReturnsAsync(todoItems);

        DateTime RealDueDate = (DateTime)await todoItemService.SetDuedate(todoItemDto, "Least Count");
        Assert.Equal(todoItemDto.CreatedDate, RealDueDate.Date);
    }

    [Fact]
    public async void Should_return_least_count_Duedate_when_create_item_auto_set_in_five_days_query_over_than_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            CreatedDate = new DateTime(2024, 10, 10)
        };
        List<TodoItemDto> todoItems = new List<TodoItemDto>();
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 8; i++)
            {
                var todoItem = new TodoItemDto
                {
                    Id = "1" + j.ToString() + i.ToString(),
                    Description = "new task",
                    IsDone = true,
                    IsFavorite = true,
                    DueDate = new DateTime(2024, 10, 10 + j)
                };
                todoItems.Add(todoItem);
            }
        }

        mockRepository.Setup(repo => repo.GetAllTodoItemsInFiveDays(It.IsAny<DateTime>())).ReturnsAsync(todoItems);

        DateTime RealDueDate = (DateTime)await todoItemService.SetDuedate(todoItemDto, "Least Count");
        DateTime ExpectedDuedate = new DateTime(2024, 10, 14);
        Assert.Equal(ExpectedDuedate.Date, RealDueDate.Date);
    }

    [Fact]
    public async void Should_return_default_Duedate_when_create_item_set_default_duedate_count_less_than_8()
    {
        var mockRepository = new Mock<ITodoItemsRepository>();
        var todoItemService = new TodoItemService(mockRepository.Object);
        var todoItemDto = new TodoItemDto
        {
            Id = "1",
            Description = "new task",
            IsDone = true,
            IsFavorite = true,
            CreatedDate = new DateTime(2024, 10, 10),
            DueDate = new DateTime(2024, 11, 11)
        };
        List<TodoItemDto> todoItems = new List<TodoItemDto>();
        for (int i = 0; i < 1; i++)
        {
            var todoItem = new TodoItemDto
            {
                Id = "1" + i.ToString(),
                Description = "new task",
                IsDone = true,
                IsFavorite = true,
                DueDate = new DateTime(2024, 10, 10)
            };
            todoItems.Add(todoItem);
        }
        mockRepository.Setup(repo => repo.GetAllTodoItemsInFiveDays(It.IsAny<DateTime>())).ReturnsAsync(todoItems);

        var RealTodoItem = await todoItemService.CreateItem(todoItemDto, "Least Count");
        Assert.Equal(todoItemDto.DueDate?.Date, RealTodoItem.DueDate?.Date);
    }
}