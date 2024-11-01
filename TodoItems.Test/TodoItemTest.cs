using ToDoItem.Api.Models;
using TodoItems.Core;
using Moq;


namespace TodoItems.Test;

public class TodoItemTest
{

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenDueDateIsBeforeCreatedTimeDate()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();

        ToDoItemObj toDoItem = new ToDoItemObj(
           id: "1",
           description: "Item 1",
           done: false,
           favorite: false,
           createdTimeDate: DateTimeOffset.Now.AddDays(-2).Date,
           lastModifiedTimeDate: DateTimeOffset.Now.Date,
           editTimes: 2,
           dueDate: DateTime.Now.AddDays(-5).Date
       );


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => toDoItem.CreateAsync(toDoItem, mockRepository.Object));
        Assert.Equal("due date cannot be before creation date", exception.Message);
    }


    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenMoreThan8ItemsExistForToday()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var toDoItemModel = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 2,
            dueDate: DateTime.Now.AddDays(2).Date
        );

        var todayItems = new List<ToDoItemObj>();
        for (int i = 0; i < 8; i++)
        {
            todayItems.Add(new ToDoItemObj(
                id: Guid.NewGuid().ToString(),
                description: $"Item {i + 1}",
                done: false,
                favorite: false,
                createdTimeDate: DateTimeOffset.Now.Date,
                lastModifiedTimeDate: DateTimeOffset.Now.Date,
                editTimes: 0,
                dueDate: DateTime.Now.AddDays(2).Date
            ));
        }

        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(todayItems);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => toDoItemModel.CreateAsync(toDoItemModel, mockRepository.Object));
        Assert.Equal("Cannot add more than 8 ToDo items for today.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnToDoItemModel_WhenDueDateIsAfterCreatedTimeDate_AndItemsAreLessThan8()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var toDoItemModel = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.AddDays(-2).Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 2,
            dueDate: DateTime.Now.AddDays(2).Date
        );

        var todayItems = new List<ToDoItemObj>();
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(todayItems);

        // Act
        var result = await toDoItemModel.CreateAsync(toDoItemModel, mockRepository.Object);

        // Assert
        Assert.Equal(toDoItemModel, result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnToDoItemModel_WhenDueDateIsAfterCreatedTimeDate()
    {
        // Arrange
        var mockRepository = new Mock<ITodosRepository>();
        var toDoItemModel = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.AddDays(-2).Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 2,
            dueDate: DateTime.Now.AddDays(2).Date
        );

        var todayItems = new List<ToDoItemObj>(); // Ensure this list is not null
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(todayItems);

        // Act
        var result = await toDoItemModel.CreateAsync(toDoItemModel, mockRepository.Object);

        // Assert
        Assert.Equal(toDoItemModel, result);
    }


    [Fact]
    public async Task should_add_1_when_edit_if_EditTimes_is_small_and_same_dayAsync()
    {
        //var mockRepository = new Mock<ITodosRepository>();
        //var items = new List<ToDoItemObj>();
        //for (int i = 0; i < 8; i++)
        //{
        //    items.Add(new ToDoItemObj(
        //        id: Guid.NewGuid().ToString(),
        //        description: $"Item {i + 1}",
        //        done: false,
        //        favorite: false,
        //        createdTimeDate: DateTimeOffset.Now.Date,
        //        lastModifiedTimeDate: DateTimeOffset.Now.Date,
        //        editTimes: 0,
        //        dueDate: DateTime.Now.AddDays(2).Date
        //    ));
        //}
        //mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        var itemNow = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 0,
            dueDate: DateTimeOffset.Now.AddDays(2).Date
        );
        ToDoItemObj itemAfterEdit = await itemNow.ModifyItem(itemNow);

        var expectedItem = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 1,
            dueDate: DateTimeOffset.Now.AddDays(2).Date
        );

        Assert.True(AreEqual(expectedItem, itemAfterEdit));
    }

    private bool AreEqual(ToDoItemObj expected, ToDoItemObj actual)
    {
        return expected.Id == actual.Id &&
               expected.Description == actual.Description &&
               expected.Done == actual.Done &&
               expected.Favorite == actual.Favorite &&
               expected.CreatedTimeDate == actual.CreatedTimeDate &&
               expected.LastModifiedTimeDate == actual.LastModifiedTimeDate &&
               expected.EditTimes == actual.EditTimes &&
               expected.DueDate == actual.DueDate;
    }

    [Fact]
    public async Task should_add_1_when_edit_if_EditTimes_is_small_and_different_day()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemObj>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemObj(
                id: Guid.NewGuid().ToString(),
                description: $"Item {i + 1}",
                done: false,
                favorite: false,
                createdTimeDate: DateTimeOffset.Now.Date,
                lastModifiedTimeDate: DateTimeOffset.Now.Date,
                editTimes: 0,
                dueDate: DateTime.Now.AddDays(2).Date
            ));
        }
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        DateTimeOffset oldDateTime = DateTimeOffset.Now.AddDays(-2);
        var itemToBeEdit = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.Date,
            lastModifiedTimeDate: oldDateTime.Date,
            editTimes: 2,
            dueDate: DateTime.Now.AddDays(2).Date
        );
        ToDoItemObj itemAfterEdit = await itemToBeEdit.ModifyItem(itemToBeEdit);

        var expectedItem = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 1, // Reset to 1 because it's a different day
            dueDate: DateTime.Now.AddDays(2).Date
        );

        Assert.True(AreEqual(expectedItem, itemAfterEdit));

    }

    [Fact]
    public async Task should_alert_when_edit_if_EditTimes_is_bigAsync()
    {
        var mockRepository = new Mock<ITodosRepository>();
        var items = new List<ToDoItemObj>();
        for (int i = 0; i < 8; i++)
        {
            items.Add(new ToDoItemObj(
                id: Guid.NewGuid().ToString(),
                description: $"Item {i + 1}",
                done: false,
                favorite: false,
                createdTimeDate: DateTimeOffset.Now.Date,
                lastModifiedTimeDate: DateTimeOffset.Now.Date,
                editTimes: 0,
                dueDate: DateTime.Now.AddDays(2).Date
            ));
        }
        mockRepository.Setup(repo => repo.findAllTodoItemsInToday()).Returns(items);

        var itemNow = new ToDoItemObj(
            id: "1",
            description: "Item 1",
            done: false,
            favorite: false,
            createdTimeDate: DateTimeOffset.Now.Date,
            lastModifiedTimeDate: DateTimeOffset.Now.Date,
            editTimes: 3,
            dueDate: DateTime.Now.AddDays(2).Date
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => itemNow.ModifyItem(itemNow));

        Assert.Equal("Too many edits", exception.Message);
    }






}