using Castle.Core.Logging;
using Moq;
using System.Runtime.ConstrainedExecution;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    private readonly ITodoItemService _service;
    private readonly ITodoItemsRepository _todoRepository;

    public TodoItemTest(){
        _service=new TodoItemService(_todoRepository);
    }

    [Fact]
    public void should_update_when_time_less_than_three()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime= [],
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
        try
        {
            var updatedItem = _service.ModifyTodoItem(oldItem, updateItem);
        }
        catch (Exception ex) { }
        Assert.Equal("should update description",updateItem.Description);
    }

    [Fact]
    public void should_throw_exception_when_time_more_than_three()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 11, 1, 00, 00, 01), new DateTime(2024, 11, 1, 00, 01, 01), new DateTime(2024, 11, 1, 01, 00, 01)],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        try
        {
            _service.ModifyTodoItem(oldItem, updateItem);
        }
        catch (Exception ex) {
            Assert.Equal(ex.Message, "No modify time");
        }
    }

    [Fact]
    public void should_update_todoItem_when_across_day()
    {
        var oldItem = new TodoItem()
        {
            Id = "1",
            Description = "old item description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 10, 30, 00, 00, 01), new DateTime(2024, 10, 30, 00, 01, 01), new DateTime(2024, 10, 30, 01, 00, 01)],
        };
        var updateItem = new TodoItem()
        {
            Id = "1",
            Description = "should update description",
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
        };
        var updatedItem = _service.ModifyTodoItem(oldItem, updateItem);

        Assert.Equal("should update description", updatedItem.Description);
        Assert.Equal(1, updatedItem.ModifyTime.Length);
    }

    //[Fact]
    //public async void should_create_success_when_has_dueDate_and_isAvailable()
    //{
    //    var _todoRepository = new Mock<ITodoItemsRepository>();
    //    var _service = new TodoItemService(_todoRepository.Object);
    //    var expectReturn = new List<TodoItem>();


    //    for (var i = 0; i < 6; i++)
    //    {
    //        expectReturn.Add(new TodoItem());
    //    }
    //    _todoRepository.Setup(repo => repo.getAllItemsCountInToday(It.IsAny<DateTime>())).ReturnsAsync(expectReturn);

    //}

    [Fact]
    public async void should_create_failed_when_item_count_morethan_eight()
    {
        var _todoRepository = new Mock<ITodoItemsRepository>();
        var _service = new TodoItemService(_todoRepository.Object);
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
    public async void should_create_success()
    {
        var _todoRepository = new Mock<ITodoItemsRepository>();
        var _service = new TodoItemService(_todoRepository.Object);
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
            CreateTime = DateTime.Now,
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [DateTime.Now.Date],
            DueTime = DateTime.Now.AddDays(3).Date,
        };

        var createToDoItem = await _service.CreateTodoItem(todoItem);
        Assert.Equal("create a new todoItem", createToDoItem.Description);
        Assert.NotEmpty(createToDoItem.Id);
    }
    [Fact]
    public async void should_create_failed_when_dueTime_earlier_createTime()
    {
        var _todoRepository = new Mock<ITodoItemsRepository>();
        var _service = new TodoItemService(_todoRepository.Object);
        var expectReturn = new List<TodoItem>();
        _todoRepository.Setup(repo => repo.getAllItemsCountInToday(It.IsAny<DateTime>())).ReturnsAsync(expectReturn);
        var todoItem = new TodoItem()
        {
            Id=Guid.NewGuid().ToString(),
            Description = "create a new todoItem",
            CreateTime = new DateTime(2024, 10, 30, 00, 00, 01),
            IsComplete = false,
            IsFavorite = false,
            ModifyTime = [new DateTime(2024, 10, 30, 00, 00, 01)],
            DueTime = DateTime.Now.Date,
        };
        var exception = await Assert.ThrowsAsync<TooManyTodoItemInDueDateException>(() => _service.CreateTodoItem(todoItem));
        Assert.Equal(exception.Message, "A maximum of eight todoitems can be completed per day");
    }
}
