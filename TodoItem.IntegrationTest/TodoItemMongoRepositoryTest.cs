using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TodoItem.Infrastructure;

namespace TodoItem.IntegrationTest;

public class TodoItemMongoRepositoryTest : IAsyncLifetime
{
    private readonly TodoItemMongoRepository _mongoRepository;
    private IMongoCollection<TodoItemDao> _mongoCollection;


    public TodoItemMongoRepositoryTest()
    {
        var mockSettings = new Mock<IOptions<TodoStoreDatabaseSettings>>();

        mockSettings.Setup(s => s.Value).Returns(new TodoStoreDatabaseSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "TodoTestStore",
            TodoItemsCollectionName = "Todos"
        });

        // ??? TodoService
        _mongoRepository = new TodoItemMongoRepository(mockSettings.Object);

        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("TodoTestStore");
        _mongoCollection = mongoDatabase.GetCollection<TodoItemDao>("Todos");
    }

    // IAsyncLifetime ?? InitializeAsync ??????????
    public async Task InitializeAsync()
    {
        // ????
        await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemDao>.Empty);
    }

    // DisposeAsync ?????????????????
    public Task DisposeAsync() => Task.CompletedTask;


    [Fact]
    public async void should_return_item_by_id_1()
    {
        var todoItemDao = new TodoItemDao
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTime.UtcNow,
            LastModifiedTimeDate = DateTime.UtcNow,
            EditTimes = 0,
            DueDate = null
        }; ;
        await _mongoCollection.InsertOneAsync(todoItemDao);
        var todoItem = await _mongoRepository.FindById("5f9a7d8e2d3b4a1eb8a7d8e2");

        Assert.NotNull(todoItem);
        Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", todoItem.Id);
        Assert.Equal("Buy groceries", todoItem.Description);
    }
}