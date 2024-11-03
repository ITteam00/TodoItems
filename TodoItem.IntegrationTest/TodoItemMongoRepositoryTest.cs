using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TodoItem.Infrastructure;

namespace TodoItem.IntegrationTest;

public class TodoItemMongoRepositoryTest: IAsyncLifetime
{
    private readonly TodoItemMongoRepository _mongoRepository;
    private IMongoCollection<TodoItemPo> _mongoCollection;


    public TodoItemMongoRepositoryTest()
    {
        var mockSettings = new Mock<IOptions<TodoStoreDatabaseSettings>>();
        
        mockSettings.Setup(s => s.Value).Returns(new TodoStoreDatabaseSettings
        {
            ConnectionString = "mongodb://localhost:27017/",
            DatabaseName = "TodoTestStore",
            TodoItemsCollectionName = "Todos"
        });

        _mongoRepository = new TodoItemMongoRepository(mockSettings.Object);
        
        var mongoClient = new MongoClient("mongodb://localhost:27017/");
        var mongoDatabase = mongoClient.GetDatabase("TodoTestStore");
        _mongoCollection = mongoDatabase.GetCollection<TodoItemPo>("Todos");
    }
    
    public async Task InitializeAsync()
    {
        await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemPo>.Empty);
    }

    public Task DisposeAsync() => Task.CompletedTask;


    [Fact]
    public async void should_return_item_by_id_1()
    {
        var todoItemPo = new TodoItemPo{
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2", 
            Description = "Buy groceries",
            IsDone = true,
            IsFavorite = true,
        };;
        await _mongoCollection.InsertOneAsync(todoItemPo);
        var todoItem = await _mongoRepository.FindById("5f9a7d8e2d3b4a1eb8a7d8e2");
        
        Assert.NotNull(todoItem);
        Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", todoItem.Id);
        Assert.Equal("Buy groceries", todoItem.Description);
    }

    [Fact]
    public async void should_return_items_In_DueDate()
    {
        var todoItemPo = new TodoItemPo
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            DueDate = new DateTime(2024, 10, 30, 10, 30, 0),
        }; 
        await _mongoCollection.InsertOneAsync(todoItemPo);
        var TodoItemCount = await _mongoRepository.GetAllTodoItemsCountInDueDate(todoItemPo.DueDate);

        Assert.Equal(1, TodoItemCount);
    }

    [Fact]
    public async void should_return_items_In_Five_Days()
    {
        for(int i = 0;i < 5; i++)
        {
            var todoItemPo = new TodoItemPo
            {
                Id = "5f9a7d8e2d3b4a1eb8a7d8e" + i.ToString(),
                Description = "Buy groceries",
                DueDate = new DateTime(2024, 10, 10+i, 10, 30, 0),
            };
            await _mongoCollection.InsertOneAsync(todoItemPo);
        }
        var CreatedDate = new DateTime(2024, 10, 10, 10, 30, 0);

        var todoItems = await _mongoRepository.GetAllTodoItemsInFiveDays(CreatedDate);

        Assert.Equal(5, todoItems.Count);
    }
    [Fact]
    public async void should_return_implement_when_save_todoitem()
    {
        var todoItemDto = new TodoItems.Core.TodoItemDto
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            IsDone = true,
            IsFavorite = true,
        }; ;
        _mongoRepository.Save(todoItemDto);
        //await _mongoCollection.InsertOneAsync(todoItemPo);

        Assert.NotNull(todoItemDto);
    }
}