using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using TodoItems.Core;
using TodoItems.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace TodoItems.Integration.Test;

public class TodoItemMongoRepositoryTest : IAsyncLifetime
{
    private readonly TodoItemMongoRepository _mongoRepository;
    private readonly string _collectionName = "TodoItems";
    private readonly string _databaseName = "TodoItemsTest";

    public TodoItemMongoRepositoryTest()
    {
        var mockSettings = new Mock<IOptions<TodoStoreDatabaseSettings>>();
        mockSettings.Setup(s => s.Value).Returns(new TodoStoreDatabaseSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = _databaseName,
            TodoItemsCollectionName = _collectionName
        });
        _mongoRepository = new TodoItemMongoRepository(mockSettings.Object);
    }

    // IAsyncLifetime 中的 InitializeAsync 方法在每个测试前运行
    public async Task InitializeAsync()
    {
        // 清空集合
        await _mongoRepository.TodosCollection.DeleteManyAsync(FilterDefinition<TodoItemDto>.Empty);
    }

    // DisposeAsync 在测试完成后运行（如果有需要的话）
    public Task DisposeAsync() => Task.CompletedTask;


    [Fact]
    public async void should_return_item_by_id_1()
    {
        var todoItemPo = new TodoItemDto
        {
            Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
            Description = "Buy groceries",
            ModificationRecord = new ModificationDto() { },
            Done = false,
            DueDate = DateTimeOffset.Now.AddDays(2),
        };
        await _mongoRepository.TodosCollection.InsertOneAsync(todoItemPo);
        var filter = Builders<TodoItemDto>.Filter
            .Eq(item => item.Id, "5f9a7d8e2d3b4a1eb8a7d8e2");

        var todoItem = _mongoRepository.TodosCollection.Find(filter).FirstOrDefault();
        Assert.NotNull(todoItem);
        Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", todoItem.Id);
        Assert.Equal("Buy groceries", todoItem.Description);
        Assert.Equal(todoItemPo.DueDate, todoItem.DueDate);
    }

    [Fact]
    public void InsertOneItem_ShouldReturnItem_WithEmptyModificationRecord()
    {
        var id = ObjectId.GenerateNewId().ToString();

        var item = new TodoItem
        {
            Id = id,
            Description = "AAA",
            ModificationRecord = new Modification() { },
            Done = true,
            DueDate = DateTimeOffset.Now.AddDays(2),
        };
        var todoItemDto = _mongoRepository.AddItem(item);

        var findResult = _mongoRepository.TodosCollection.Find(x => x.Id == todoItemDto.Id).FirstOrDefault();
        Assert.NotNull(findResult);
        Assert.Equal(todoItemDto.Id, findResult.Id);
        Assert.Equal(todoItemDto.Description, findResult.Description);
        Assert.Equal(todoItemDto.Done, findResult.Done);
        Assert.Equal(todoItemDto.ModificationRecord.ModifiedTimes, findResult.ModificationRecord.ModifiedTimes);
        Assert.Equal(todoItemDto.DueDate, findResult.DueDate);
    }

    [Fact]
    public void InsertOneItem_ShouldReturnItem_WithModificationRecord()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var item = new TodoItem
        {
            Id = id,
            Description = "Test with modifications",
            ModificationRecord = new Modification
            {
                ModifiedTimes = new List<DateTimeOffset> { DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1) }
            },
            Done = false
        };

        // Act
        _mongoRepository.AddItem(item);
        var filter = Builders<TodoItemDto>.Filter.Eq(itee => itee.Id, id);
        var findResult = _mongoRepository.TodosCollection.Find(filter).FirstOrDefault();

        // Assert
        Assert.NotNull(findResult);
        Assert.Equal(item.Id, findResult.Id);
        Assert.Equal(item.Description, findResult.Description);
        Assert.Equal(item.Done, findResult.Done);
        Assert.Equal(item.ModificationRecord.ModifiedTimes.Count, findResult.ModificationRecord.ModifiedTimes.Count);
        Assert.True(item.ModificationRecord.ModifiedTimes.SequenceEqual(findResult.ModificationRecord.ModifiedTimes));
        Assert.Equal(item.ModificationRecord.ModifiedTimes, findResult.ModificationRecord.ModifiedTimes);
    }

    [Fact]
    public void GetItemById_ShouldReturnItemWithId()
    {
        var id = ObjectId.GenerateNewId().ToString();
        var item = new TodoItem
        {
            Id = id,
            Description = "Test with modifications",
            ModificationRecord = new Modification(),
            Done = false
        };
        var items = new List<TodoItem>
        {
            new TodoItem()
                { Id = ObjectId.GenerateNewId().ToString(), Description = "aaa", ModificationRecord = new() },
            new TodoItem()
                { Id = ObjectId.GenerateNewId().ToString(), Description = "bbb", ModificationRecord = new() },
            item
        };
        foreach (var i in items)
        {
            _mongoRepository.AddItem(i);
        }

        var findResult = _mongoRepository.GetItemById(id);
        Assert.NotNull(findResult);
        Assert.Equal(item.Id, findResult.Id);
        Assert.Equal(item.Description, findResult.Description);
    }

    [Fact]
    public void InsertOneItem()
    {
        var dto = new TodoItemDto() { Description = "jjjjjjjjjjj" };
        _mongoRepository.TodosCollection.InsertOne(dto);
        var findResult = _mongoRepository.TodosCollection.Find(x => x.Description == dto.Description);
        Assert.Equal(dto.Description, findResult.FirstOrDefault().Description);
    }

    [Fact]
    public void GetItemsByDueDate_ShouldReturnItems_WithDueDate()
    {

        var items = new List<TodoItem>
        {
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Description = "aaa", 
                ModificationRecord = new(),
                DueDate = DateTimeOffset.Now.AddDays(1),
            },
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                Description = "bbb", 
                ModificationRecord = new(),
                DueDate = DateTimeOffset.Now.AddDays(2),
            },
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Description = "ccc", 
                ModificationRecord = new(),
                DueDate = DateTimeOffset.Now.AddDays(2),
            },
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                Description = "ddd", 
                ModificationRecord = new(),
                DueDate = DateTimeOffset.Now.AddDays(3),
            },
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                Description = "eee", 
                ModificationRecord = new(),
            },
        };
        foreach (var i in items)
        {
            _mongoRepository.AddItem(i);
        }
        var result = _mongoRepository.GetItemsByDueDate(DateTimeOffset.Now.AddDays(2));
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public void GetItemsByDueDate_ShouldReturnNull()
    {

        var items = new List<TodoItem>
        {
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Description = "aaa", 
                ModificationRecord = new(),
            },
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                Description = "bbb", 
                ModificationRecord = new(),
            },
            new TodoItem()
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                Description = "ddd", 
                ModificationRecord = new(),
                DueDate = DateTimeOffset.Now.AddDays(3),
            },

        };
        foreach (var i in items)
        {
            _mongoRepository.AddItem(i);
        }
        var result = _mongoRepository.GetItemsByDueDate(DateTimeOffset.Now.AddDays(2));
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    
    [Fact]
    public void GetFiveDayItems_ShouldReturnItemsWithinFiveDays()
    {
        // Arrange
        var today = DateTimeOffset.Now.Date;
        var fifthDay = today.AddDays(5);

        var items = new List<TodoItem>
        {
            new TodoItem { Id = ObjectId.GenerateNewId().ToString(),  Description = "Item 1", DueDate = today.AddHours(1)},
            new TodoItem { Id = ObjectId.GenerateNewId().ToString(),  Description = "Item 2", DueDate = fifthDay.AddHours(23) },
            new TodoItem { Id = ObjectId.GenerateNewId().ToString(),  Description = "Item 3", DueDate = today.AddDays(1) }, 
            new TodoItem { Id = ObjectId.GenerateNewId().ToString(),  Description = "Item 4", DueDate = today.AddDays(7) }, 
            new TodoItem { Id = ObjectId.GenerateNewId().ToString(),  Description = "Item 5"}, 
            new TodoItem { Id = ObjectId.GenerateNewId().ToString(),  Description = "Item 6", DueDate = fifthDay.AddHours(25) },
        };
        foreach (var i in items)
        {
            _mongoRepository.AddItem(i);
        }
        // Act
        var result = _mongoRepository.GetFiveDayItems(DateTimeOffset.Now);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, item => item.Description == "Item 1");
        Assert.Contains(result, item => item.Description == "Item 2");
        Assert.Contains(result, item => item.Description == "Item 3");
        Assert.Equal("Item 3", result.ToList()[1].Description);  // sort works
    }

}