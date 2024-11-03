using Castle.Core.Logging;
using Microsoft.VisualBasic;
using Moq;
using System.Runtime.ConstrainedExecution;
using TodoItems.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoItems.Test;

public class TodoItemTest
{
    private readonly ITodoItemService _service;
    private readonly Mock<ITodoItemsRepository> _todoRepository;

    public TodoItemTest(){
        _todoRepository = new Mock<ITodoItemsRepository>();
        _service = new TodoItemService(_todoRepository.Object);
    }

    [Fact]
    public async void should_update_when_time_less_than_three()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 11, 1, 00, 00, 01)],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [],
        };
        _todoRepository.Setup(repo => repo.FindById(It.IsAny<string>())).ReturnsAsync(oldItem);
        var updatedItem = await _service.ModifyTodoItem("1", updateItem);
        Assert.Equal("should update description",updateItem.Description);
    }

    [Fact]
    public async void should_failed_throw_exception_when_time_more_than_three()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [DateTime.Now, DateTime.Now, DateTime.Now],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        _todoRepository.Setup(repo => repo.FindById(It.IsAny<string>())).ReturnsAsync(oldItem);
        var exception = await Assert.ThrowsAsync<NoModifyTimeException>(() => _service.ModifyTodoItem("1",updateItem));
        _todoRepository.Verify(repo => repo.SaveAsync(oldItem), Times.Never);
    }

    [Fact]
    public async void should_update_todoItem_when_across_day()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-3)],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        _todoRepository.Setup(repo => repo.FindById(It.IsAny<string>())).ReturnsAsync(oldItem);
        var updatedItem = await _service.ModifyTodoItem("1", updateItem);

        Assert.Equal("should update description", updatedItem.Description);
        Assert.Equal(1, updatedItem.ModifyTime.Length);
    }

    [Fact]
    public async void should_create_failed_when_item_count_morethan_eight()
    {
        var expectReturn = new List<TodoItem>();
        for (var i = 0; i < 9; i++)
        {
            expectReturn.Add(new TodoItem());
        }
        _todoRepository.Setup(repo => repo.getAllItemsCountInToday(It.IsAny<DateTime>())).ReturnsAsync(expectReturn);
        var todoItem = new TodoItem()
        {
            Id=Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            DueTime = DateTime.Now,
        };
        var exception= await Assert.ThrowsAsync<TooManyTodoItemInDueDateException>(()=> _service.CreateTodoItem(todoItem)) ;
        Assert.Equal(exception.Message, "A maximum of eight todoitems can be completed per day");
    }

    [Fact]
    public async void should_create_success_when_have_special_available_dueTime()
    {
        var expectReturn = new List<TodoItem>();
        for (var i = 0; i < 3; i++)
        {
            expectReturn.Add(new TodoItem());
        }
        _todoRepository.Setup(repo => repo.getAllItemsCountInToday(It.IsAny<DateTime>())).ReturnsAsync(expectReturn);
        var todoItem = new TodoItem()
        {
            Id=Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [DateTime.Now.Date],
            DueTime = DateTime.Now.AddDays(3).Date,
        };

        var createToDoItem = await _service.CreateTodoItem(todoItem);
        Assert.Equal("create a new todoItem", createToDoItem.Description);
        _todoRepository.Verify(repo => repo.SaveAsync(todoItem), Times.Once);

        Assert.NotEmpty(createToDoItem.Id);
    }
    [Fact]
    public async void should_create_failed_when_dueTime_earlier_createTime()
    {
        var expectReturn = new List<TodoItem>();
        _todoRepository.Setup(repo => repo.getAllItemsCountInToday(It.IsAny<DateTime>())).ReturnsAsync(expectReturn);
        var todoItem = new TodoItem()
        {
            Id=Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 10, 30, 00, 00, 01)],
            DueTime = DateTime.Now.AddDays(-2).Date,
        };
        var exception = await Assert.ThrowsAsync<DueDateEarlierThanCreateDateException>(() => _service.CreateTodoItem(todoItem));
        Assert.Equal(exception.Message, "Due time should later than create time");
    }

    [Fact]
    public async void dueDate_should_be_second_day_of_today()
    {
        var todoItem = new TodoItem()
        {
            Id = Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        string strategytype = "freest";
        DateTime today = DateTime.Now.Date;
        List<TodoItem> nextFiveDaysItems = [];
        for (int i = 1; i < 6; i++)
        {
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
        }
        nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(1) });
        _todoRepository.Setup(repo => repo.getNextFiveDaysItem(It.IsAny<DateTime>())).ReturnsAsync(nextFiveDaysItems);
        var createdTodoItem= await _service.CreateTodoItem(todoItem,strategytype);
        Assert.Equal(today.AddDays(2), createdTodoItem.DueTime);
        _todoRepository.Verify(repo => repo.SaveAsync(todoItem), Times.Once);
    }

    [Fact]
    public async void dueDate_should_be_first_day_of_today()
    {
        var todoItem = new TodoItem()
        {
            Id = Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        string strategytype = "closest";
        DateTime today = DateTime.Now.Date;
        List<TodoItem> nextFiveDaysItems = [];
        for (int i = 1; i < 6; i++)
        {
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
        }
        nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(1) });
        _todoRepository.Setup(repo => repo.getNextFiveDaysItem(It.IsAny<DateTime>())).ReturnsAsync(nextFiveDaysItems);
        var createdTodoItem = await _service.CreateTodoItem(todoItem, strategytype);
        Assert.Equal(today.AddDays(1), createdTodoItem.DueTime);
        _todoRepository.Verify(repo => repo.SaveAsync(todoItem), Times.Once);
    }

    [Fact]
    public async void should_throw_Invalid_daueDate_generate_strategy_exception()
    {
        var todoItem = new TodoItem()
        {
            Id = Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        string strategytype = "invalid strategy";
        DateTime today = DateTime.Now.Date;
        List<TodoItem> nextFiveDaysItems = [];
        for (int i = 1; i < 6; i++)
        {
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
        }
        nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(1) });
        _todoRepository.Setup(repo => repo.getNextFiveDaysItem(It.IsAny<DateTime>())).ReturnsAsync(nextFiveDaysItems);
        var exception = await Assert.ThrowsAsync<InvalidDueDateGenerateStrategyException>(() => _service.CreateTodoItem(todoItem,strategytype));
        Assert.Equal(exception.Message, "Invalid daueDate generate strategy");
        _todoRepository.Verify(repo => repo.SaveAsync(todoItem), Times.Never);

    }

    [Fact]
    public async void should_throw_No_Suitable_DueDate_Exception()
    {
        var todoItem = new TodoItem()
        {
            Id = Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        string strategytype = "freest";
        DateTime today = DateTime.Now.Date;
        List<TodoItem> nextFiveDaysItems = [];
        for (int i = 1; i < 6; i++)
        {
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });
            nextFiveDaysItems.Add(new TodoItem() { DueTime = today.AddDays(i) });

        }
        _todoRepository.Setup(repo => repo.getNextFiveDaysItem(It.IsAny<DateTime>())).ReturnsAsync(nextFiveDaysItems);
        var exception = await Assert.ThrowsAsync<NoSuitableDueDateException>(() => _service.CreateTodoItem(todoItem,strategytype));
        Assert.Equal(exception.Message, "No suitable due date found");
        _todoRepository.Verify(repo => repo.SaveAsync(todoItem),Times.Never);
    }


}
