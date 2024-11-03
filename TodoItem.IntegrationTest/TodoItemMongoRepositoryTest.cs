using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TodoItem.Infrastructure;
using TodoItems.Core.Model;

namespace TodoItem.IntegrationTest;

public class TodoItemMongoRepositoryTest : IAsyncLifetime
{
    private readonly TodoItemMongoRepository _mongoRepository;
    private IMongoCollection<TodoItemPo> _mongoCollection;


    public TodoItemMongoRepositoryTest()
    {
        var mockSettings = new Mock<IOptions<TodoStoreDatabaseSettings>>();

        mockSettings.Setup(s => s.Value).Returns(new TodoStoreDatabaseSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "TodoItemTest",
            TodoItemsCollectionName = "TodoItemTest"
        });

        // 初始化 TodoService
        _mongoRepository = new TodoItemMongoRepository(mockSettings.Object);

        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("TodoItemTest");
        _mongoCollection = mongoDatabase.GetCollection<TodoItemPo>("TodoItemTest");
    }

    // IAsyncLifetime 中的 InitializeAsync 方法在每个测试前运行
    public async Task InitializeAsync()
    {
        // 清空集合
        await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemPo>.Empty);
    }

    // DisposeAsync 在测试完成后运行（如果有需要的话）
    public Task DisposeAsync() => Task.CompletedTask;


    [Fact]
    public async void should_return_item_by_id_1()
    {
        var todoItemPo = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            IsComplete = false
        }; ;
        await _mongoCollection.InsertOneAsync(todoItemPo);
        var todoItem = await _mongoRepository.FindById("5f9a7d8e2d3b4a1eb8a7d8e2");

        Assert.NotNull(todoItem);
        Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", todoItem.Id);
        Assert.Equal("Buy groceries", todoItem.Description);
    }

    [Fact]
    public async void should_return_items_when_have_duedate()
    {
        var dueDate = DateTime.Now.Date;
        TodoItemPo todoItemOne = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e3",
            DueDate = dueDate
        };
        TodoItemPo todoItemTwo = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e4",
            DueDate = dueDate
        };
        TodoItemPo todoItemThree = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e5",
            DueDate = dueDate.AddDays(1)
        };
        await _mongoCollection.InsertOneAsync(todoItemOne);
        await _mongoCollection.InsertOneAsync(todoItemTwo);
        await _mongoCollection.InsertOneAsync(todoItemThree);
        var result=await _mongoRepository.GetAllItemsInDueDate(dueDate);
        Assert.Equal(2,result.Count());
        Assert.All(result, item => Assert.Equal(dueDate, item.DueDate));
    }

    [Fact]
    public async void should_update_success()
    {
        var todoItem = new TodoItems.Core.Model.TodoItem
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            IsComplete = false
        };
        await _mongoRepository.CreateTodoItemAsync(todoItem);

        var updateTodoItem = new TodoItems.Core.Model.TodoItem
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "update description",
            IsComplete = false
        };

        await _mongoRepository.UpdateAsync(todoItem.Id, updateTodoItem);

        var result=await _mongoRepository.FindById(todoItem.Id);
        Assert.Equal("update description", result.Description);




    }
}