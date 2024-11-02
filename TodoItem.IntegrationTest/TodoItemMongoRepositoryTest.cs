using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TodoItem.Infrastructure;
using ToDoItem.Api.Models;

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
            CreatedTimeDate = DateTime.UtcNow.Date,
            LastModifiedTimeDate = DateTime.UtcNow.Date,
            EditTimes = 0,
            DueDate = null
        }; ;
        await _mongoCollection.InsertOneAsync(todoItemDao);
        var todoItem = await _mongoRepository.FindById("5f9a7d8e2d3b4a1eb8a7d8e2");

        Assert.NotNull(todoItem);
        Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", todoItem.Id);
        Assert.Equal("Buy groceries", todoItem.Description);
    }


    [Fact]
    public async void should_save_item_with_new_values()
    {
        var todoItemDao = new TodoItemDao
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e3",
            Description = "Buy goods",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTime.UtcNow.Date,
            LastModifiedTimeDate = DateTime.UtcNow.AddDays(-2).Date,
            EditTimes = 0,
            DueDate = DateTime.UtcNow.AddDays(2).Date
        }; 
        await _mongoCollection.InsertOneAsync(todoItemDao);

        var newTodoItemObj = new ToDoItemObj
        (
            "5f9a7d8e2d3b4a1eb8a7d8e3",
            "Buy goods",
            true,
            false,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            1,
            DateTime.UtcNow.Date
        );
        var res = await _mongoRepository.Save(newTodoItemObj);
        var updatedTodoItem = await _mongoRepository.FindById("5f9a7d8e2d3b4a1eb8a7d8e3");
        var expectedToDoItemObj = new ToDoItemObj
        (
            "5f9a7d8e2d3b4a1eb8a7d8e3",
            "Buy goods",
            true,
            false,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            1,
            DateTime.UtcNow.Date
        );


        Assert.NotNull(updatedTodoItem);
        Assert.Equal(expectedToDoItemObj.Id, updatedTodoItem.Id);
        Assert.Equal(expectedToDoItemObj.EditTimes, updatedTodoItem.EditTimes);
        Assert.Equal(expectedToDoItemObj.LastModifiedTimeDate, updatedTodoItem.LastModifiedTimeDate);

    }
}