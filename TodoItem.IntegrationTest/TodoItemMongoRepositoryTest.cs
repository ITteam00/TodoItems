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

        _mongoRepository = new TodoItemMongoRepository(mockSettings.Object);

        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var mongoDatabase = mongoClient.GetDatabase("TodoTestStore");
        _mongoCollection = mongoDatabase.GetCollection<TodoItemDao>("Todos");
    }

    public async Task InitializeAsync()
    {
        await _mongoCollection.DeleteManyAsync(FilterDefinition<TodoItemDao>.Empty);
    }

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
            DateTime.UtcNow.Date,
            DueDateRequirementType.Earliest
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
            DateTime.UtcNow.Date,
            DueDateRequirementType.Earliest
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
        new ToDoItemObj(todoItemDao1.Id, todoItemDao1.Description, todoItemDao1.Done, todoItemDao1.Favorite, todoItemDao1.CreatedTimeDate, todoItemDao1.LastModifiedTimeDate, todoItemDao1.EditTimes, todoItemDao1.DueDate, DueDateRequirementType.Earliest),
        new ToDoItemObj(todoItemDao2.Id, todoItemDao2.Description, todoItemDao2.Done, todoItemDao2.Favorite, todoItemDao2.CreatedTimeDate, todoItemDao2.LastModifiedTimeDate, todoItemDao2.EditTimes, todoItemDao2.DueDate, DueDateRequirementType.Earliest)
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
            DateTime.UtcNow.AddDays(5).Date,
            DueDateRequirementType.Earliest
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

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenDueDateIsBeforeCreatedTimeDate()
    {
        var todoItem = new ToDoItemObj(
            id: "1",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.Now,
            lastModifiedTimeDate: DateTime.Now,
            editTimes: 0,
            dueDate: DateTime.Now.AddDays(-1) ,
            DueDateRequirementType.Earliest
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mongoRepository.CreateAsync(todoItem));
        Assert.Equal("Due date cannot be before creation date", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenMoreThan8ItemsExistForToday()
    {
        for (int i = 0; i < 8; i++)
        {
            var todoItemDao = new TodoItemDao
            {
                Id = $"{2+i}f9a7d8e1d3b4a1eb8a7d8e8",
                Description = $"Test ToDo Item {i}",
                Done = false,
                Favorite = false,
                CreatedTimeDate = DateTime.UtcNow.Date,
                LastModifiedTimeDate = DateTime.UtcNow.Date,
                EditTimes = 0,
                DueDate = DateTime.UtcNow.Date
            };
            await _mongoCollection.InsertOneAsync(todoItemDao);
        }

        var newTodoItem = new ToDoItemObj(
            id: "1f9a7d8e2d3b4a1eb8a7d8e8",
            description: "New ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 0,
            dueDate: DateTime.UtcNow.Date,
            DueDateRequirementType.Earliest
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mongoRepository.CreateAsync(newTodoItem));
        Assert.Equal("Cannot add more than 8 ToDo items for today.", exception.Message);
    }


    [Fact]
    public async Task CreateAsync_ShouldReturnToDoItemModel_WhenDueDateIsAfterCreatedTimeDate_AndItemsAreLessThan8()
    {
        var newTodoItem = new ToDoItemObj(
            id: "3f9a7d8e2d3b4a1eb8a7d8e8",
            description: "New ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 0,
            dueDate: DateTime.UtcNow.AddDays(1).Date,
            DueDateRequirementType.Earliest
        );

        var result = await _mongoRepository.CreateAsync(newTodoItem);

        Assert.NotNull(result);
        Assert.Equal(newTodoItem.Id, result.Id);
        Assert.Equal(newTodoItem.Description, result.Description);
        Assert.Equal(newTodoItem.DueDate, result.DueDate);
    }



    [Fact]
    public async Task should_add_1_when_edit_if_EditTimes_is_small_and_same_dayAsync()
    {
        // Arrange
        var todoItem = new ToDoItemObj(
            id: "3f9a7d8e2d3b4a1eb8a7d8e8",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 1,
            dueDate: DateTime.UtcNow.AddDays(1).Date,
            DueDateRequirementType.Earliest
        );

        await _mongoRepository.Save(todoItem);

        // Act
        var updatedItem = await _mongoRepository.ModifyItem(todoItem);

        // Assert
        Assert.Equal(2, updatedItem.EditTimes);
    }

    [Fact]
    public async Task should_return_1_when_edit_if_EditTimes_is_big_but_different_day()
    {
        // Arrange
        var todoItem = new ToDoItemObj(
            id: "3f9a7d8e2d3b4a1eb8a7d8e8",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.AddDays(-2).Date,
            lastModifiedTimeDate: DateTime.UtcNow.AddDays(-2).Date,
            editTimes: 3,
            dueDate: DateTime.UtcNow.AddDays(1).Date,
            DueDateRequirementType.Earliest
        );

        await _mongoRepository.Save(todoItem);

        // Act
        var updatedItem = await _mongoRepository.ModifyItem(todoItem);

        // Assert
        Assert.Equal(1, updatedItem.EditTimes);
    }

    [Fact]
    public async Task should_Throw_when_edit_if_EditTimes_is_big()
    {
        // Arrange
        var todoItem = new ToDoItemObj(
            id: "3f9a7d8e2d3b4a1eb8a7d8e8",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 3,
            dueDate: DateTime.UtcNow.AddDays(1).Date,
            dueDateRequirement: DueDateRequirementType.Earliest
        );

        await _mongoRepository.Save(todoItem);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mongoRepository.ModifyItem(todoItem));
        Assert.Equal("Too many edits", exception.Message);
    }

    [Fact]
    public async Task should_Throw_when_InputDueDateAndRequirementTypeAreAllEmpty()
    {
        // Arrange
        var todoItem = new ToDoItemObj(
            id: "4f9a7d8e2d3b4a1eb8a7d8e8",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 3,
            dueDate: null,
            dueDateRequirement: null

        );


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mongoRepository.CreateAsync(todoItem));
        Assert.Equal("Due Date and DueDateRequirement cannot be empty at the same time.", exception.Message);
    }

    [Fact]
    public async Task should_ChooseDueDate_When_DueDateAndRequirementBothExist()
    {
        // Arrange
        var inputTodoItem = new ToDoItemObj(
            id: "5f9a7d8e2d3b4a1eb8a7d8e8",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 3,
            dueDate: DateTime.UtcNow.AddDays(4).Date, 
            dueDateRequirement: DueDateRequirementType.Earliest
        );


        // Act
        var result = await _mongoRepository.CreateAsync(inputTodoItem);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputTodoItem.DueDate, result.DueDate); 


    }


    [Fact]
    public async Task should_ChooseEarliestDate_WhenNoDueDate()
    {
        //????????????????5????????item????8??????todoItem?duedate??????
        //??5?item?????8??throw error
        // Arrange
        var inputTodoItem = new ToDoItemObj(
            id: "6f9a7d8e2d3b4a1eb8a7d8e9",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 1,
            dueDate: null,
            dueDateRequirement: DueDateRequirementType.Earliest
        );

        for (int i = 0; i < 8; i++)
        {
            var todoItem = new ToDoItemObj(
                id: $"6f9a7d8e{2+i}d3b4a1eb8a7d8e9",
                description: $"Test ToDo Item {i}",
                done: false,
                favorite: false,
                createdTimeDate: DateTime.UtcNow.Date,
                lastModifiedTimeDate: DateTime.UtcNow.Date,
                editTimes: 0,
                dueDate: DateTime.UtcNow.Date, 
                dueDateRequirement: null
            );
            await _mongoRepository.CreateAsync(todoItem);
        }


        // Act
        var result = await _mongoRepository.CreateAsync(inputTodoItem);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.DueDate);
        Assert.Equal(DateTime.UtcNow.AddDays(1).Date, result.DueDate);
    }

    [Fact]
    public async Task should_ChooseFewestDate_WhenNoDueDate()
    {
        // Arrange
        var inputTodoItem = new ToDoItemObj(
            id: "5f9a7d8e2d3b4a1eb8a7d8ea",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 3,
            dueDate: null,
            dueDateRequirement: DueDateRequirementType.Fewest
        );

        for (int i = 0; i < 7; i++)
        {
            var todoItem = new ToDoItemObj(
                id: $"6f9a7d8e2d{2+i}b4a1eb8a7d8e9",
                description: $"Test ToDo Item {i}",
                done: false,
                favorite: false,
                createdTimeDate: DateTime.UtcNow.Date,
                lastModifiedTimeDate: DateTime.UtcNow.Date,
                editTimes: 0,
                dueDate: DateTime.UtcNow.Date,
                dueDateRequirement: null
            );
            await _mongoRepository.CreateAsync(todoItem);
        }

        // Act
        var result = await _mongoRepository.CreateAsync(inputTodoItem);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.DueDate);
        Assert.Equal(DateTime.UtcNow.AddDays(1).Date, result.DueDate);

    }

    [Fact]
    public async Task should_ThrowError_WhenNext5DdaysAllHave8Items()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var todoItem = new ToDoItemObj(
                    id: Guid.NewGuid().ToString(),
                    description: $"Test ToDo Item {i}-{j}",
                    done: false,
                    favorite: false,
                    createdTimeDate: DateTime.UtcNow.Date,
                    lastModifiedTimeDate: DateTime.UtcNow.Date,
                    editTimes: 0,
                    dueDate: DateTime.UtcNow.AddDays(i).Date, // ?? DueDate ????5?
                    dueDateRequirement: null
                );
                await _mongoRepository.CreateAsync(todoItem);
            }
        }

        var newTodoItem = new ToDoItemObj(
            id: "5f9a7d8e2d3b4a1eb8a7d8eb",
            description: "Test ToDo Item",
            done: false,
            favorite: false,
            createdTimeDate: DateTime.UtcNow.Date,
            lastModifiedTimeDate: DateTime.UtcNow.Date,
            editTimes: 3,
            dueDate: null,
            dueDateRequirement: DueDateRequirementType.Earliest
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _mongoRepository.CreateAsync(newTodoItem));
        Assert.Equal("Cannot add more than 8 ToDo items for today.", exception.Message);
    }

}