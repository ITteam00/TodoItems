using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
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
            DueDate = DateTime.UtcNow.Date
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


    [Fact]
    public async void should_findAllTodoItemsInToday()
    {
        var todoItemDao1 = new TodoItemDao
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e4",
            Description = "Buy goods",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTime.UtcNow.Date,
            LastModifiedTimeDate = DateTime.UtcNow.AddDays(-2).Date,
            EditTimes = 0,
            DueDate = DateTime.UtcNow.AddDays(2).Date
        };
        var todoItemDao2 = new TodoItemDao
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e5",
            Description = "Buy goods",
            Done = false,
            Favorite = false,
            CreatedTimeDate = DateTime.UtcNow.Date,
            LastModifiedTimeDate = DateTime.UtcNow.AddDays(-2).Date,
            EditTimes = 0,
            DueDate = DateTime.UtcNow.AddDays(2).Date
        };
        await _mongoCollection.InsertOneAsync(todoItemDao1);
        await _mongoCollection.InsertOneAsync(todoItemDao2);

        var toDoItemsList = _mongoRepository.findAllTodoItemsInOneday(DateTime.UtcNow.AddDays(2).Date);

        var expectedToDoItemObjs = new List<ToDoItemObj>
    {
        new ToDoItemObj(todoItemDao1.Id, todoItemDao1.Description, todoItemDao1.Done, todoItemDao1.Favorite, todoItemDao1.CreatedTimeDate, todoItemDao1.LastModifiedTimeDate, todoItemDao1.EditTimes, todoItemDao1.DueDate),
        new ToDoItemObj(todoItemDao2.Id, todoItemDao2.Description, todoItemDao2.Done, todoItemDao2.Favorite, todoItemDao2.CreatedTimeDate, todoItemDao2.LastModifiedTimeDate, todoItemDao2.EditTimes, todoItemDao2.DueDate)
    };

        Assert.NotNull(toDoItemsList);
        Assert.Equal(expectedToDoItemObjs.Count, toDoItemsList.Count);
        for (int i = 0; i < expectedToDoItemObjs.Count; i++)
        {
            Assert.Equal(expectedToDoItemObjs[i].Id, toDoItemsList[i].Id);
            Assert.Equal(expectedToDoItemObjs[i].Description, toDoItemsList[i].Description);
            Assert.Equal(expectedToDoItemObjs[i].Done, toDoItemsList[i].Done);
            Assert.Equal(expectedToDoItemObjs[i].Favorite, toDoItemsList[i].Favorite);
            Assert.Equal(expectedToDoItemObjs[i].CreatedTimeDate, toDoItemsList[i].CreatedTimeDate);
            Assert.Equal(expectedToDoItemObjs[i].LastModifiedTimeDate, toDoItemsList[i].LastModifiedTimeDate);
            Assert.Equal(expectedToDoItemObjs[i].EditTimes, toDoItemsList[i].EditTimes);
            Assert.Equal(expectedToDoItemObjs[i].DueDate, toDoItemsList[i].DueDate);
        }
    }


    [Fact]
    public async void should_create_new_todoItem()
    {
        var newTodoItemObj = new ToDoItemObj
        (
            "5f9a7d8e2d3b4a1eb8a7d8e7",
            "Buy goods",
            true,
            false,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            1,
            DateTime.UtcNow.AddDays(5).Date
        );
        var res = await _mongoRepository.Save(newTodoItemObj);
        var todoItemInRepo = await _mongoRepository.FindById("5f9a7d8e2d3b4a1eb8a7d8e7");


        Assert.NotNull(todoItemInRepo);
        Assert.Equal(todoItemInRepo.Id, newTodoItemObj.Id);
        Assert.Equal(todoItemInRepo.Description, newTodoItemObj.Description);
        Assert.Equal(todoItemInRepo.Done, newTodoItemObj.Done);
        Assert.Equal(todoItemInRepo.Favorite, newTodoItemObj.Favorite);
        Assert.Equal(todoItemInRepo.CreatedTimeDate, newTodoItemObj.CreatedTimeDate);
        Assert.Equal(todoItemInRepo.LastModifiedTimeDate, newTodoItemObj.LastModifiedTimeDate);
        Assert.Equal(todoItemInRepo.EditTimes, newTodoItemObj.EditTimes);
        Assert.Equal(todoItemInRepo.DueDate, newTodoItemObj.DueDate);

    }


}