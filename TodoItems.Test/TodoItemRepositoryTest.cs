using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;
using TodoItems.Core.Services;
using TodoItems.Core;
using Xunit;

namespace TodoItems.Test
{
    public class TodoItemRepositoryTest
    {
        private readonly Mock<IMongoCollection<TodoItemMongoDTO>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IOptions<ToDoItemDatabaseSettings>> _mockSettings;
        private readonly Mock<ILogger<ToDoItemsService>> _mockLogger;
        private readonly TodoItemsRepository _repository;

        public TodoItemRepositoryTest()
        {
            _mockCollection = new Mock<IMongoCollection<TodoItemMongoDTO>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
            _mockSettings = new Mock<IOptions<ToDoItemDatabaseSettings>>();
            _mockLogger = new Mock<ILogger<ToDoItemsService>>();

            var settings = new ToDoItemDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27081",
                DatabaseName = "TestTodoDatabase",
                CollectionName = "TodoItems"
            };
            _mockSettings.Setup(s => s.Value).Returns(settings);

            _mockClient.Setup(c => c.GetDatabase(settings.DatabaseName, null)).Returns(_mockDatabase.Object);
            _mockDatabase.Setup(d => d.GetCollection<TodoItemMongoDTO>(settings.CollectionName, null))
                .Returns(_mockCollection.Object);

            _repository = new TodoItemsRepository(_mockSettings.Object, _mockLogger.Object);
        }

        public async Task InitializeAsync()
        {
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetItemsByDueDate_ReturnsCorrectItems()
        {
            var dueDate = DateTimeOffset.Now.Date;
            var todoItems = new List<TodoItemMongoDTO>
            {
                new TodoItemMongoDTO { Id = "1", Description = "Test 1", DueDate = dueDate },
                new TodoItemMongoDTO { Id = "2", Description = "Test 2", DueDate = dueDate }
            };

            var mockCursor = new Mock<IAsyncCursor<TodoItemMongoDTO>>();
            mockCursor.Setup(_ => _.Current).Returns(todoItems);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<TodoItemMongoDTO>>(),
                    It.IsAny<FindOptions<TodoItemMongoDTO, TodoItemMongoDTO>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var result = await _repository.GetItemsByDueDate(dueDate);

            Assert.Equal(2, result.Count);
            Assert.Equal("Test 1", result[0].Description);
            Assert.Equal("Test 2", result[1].Description);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReplaceOneAsync()
        {
            var id = "testId";
            var updatedTodoItem = new TodoItemDTO()
            {
                Id = id,
                Description = "Updated Description",
                IsDone = true,
                IsFavorite = false,
                CreatedTime = DateTime.UtcNow
            };
            await _repository.UpdateAsync(id, updatedTodoItem);
            Assert.NotNull(updatedTodoItem);
        }

        [Fact]
        public async Task CreateAsync_Should_Insert_New_Item()
        {
            // Arrange
            var newTodoItem = new TodoItemDTO
            {
                Id = "testId",
                Description = "Test Description",
                IsDone = false,
                IsFavorite = true,
                CreatedTime = DateTime.UtcNow
            };

            var item = new TodoItemMongoDTO
            {
                Id = newTodoItem.Id,
                Description = newTodoItem.Description,
                IsDone = newTodoItem.IsDone,
                IsFavorite = newTodoItem.IsFavorite,
                CreatedTime = newTodoItem.CreatedTime,
            };

            await _repository.CreateAsync(newTodoItem);

            _mockCollection.Verify(c => c.InsertOneAsync(It.Is<TodoItemMongoDTO>(i => i.Id == item.Id &&
                        i.Description == item.Description &&
                        i.IsDone == item.IsDone &&
                        i.IsFavorite == item.IsFavorite &&
                        i.CreatedTime == item.CreatedTime),
                    null,
                    default),
                Times.Once);

            _mockLogger.Verify(l => l.LogInformation($"Inserting new todo item to DB {newTodoItem.Id}"), Times.Once);
        }


        [Fact]
        public async Task GetNextFiveDaysItems_ReturnsCorrectItems()
        {
            // Arrange
            var startDate = DateTimeOffset.Now.Date;
            var todoItems = new List<TodoItemMongoDTO>
            {
                new TodoItemMongoDTO { Id = "1", Description = "Test 1", DueDate = startDate.AddDays(1) },
                new TodoItemMongoDTO { Id = "2", Description = "Test 2", DueDate = startDate.AddDays(2) }
            };

            var mockCursor = new Mock<IAsyncCursor<TodoItemMongoDTO>>();
            mockCursor.Setup(_ => _.Current).Returns(todoItems);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<TodoItemMongoDTO>>(),
                    It.IsAny<FindOptions<TodoItemMongoDTO, TodoItemMongoDTO>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var result = await _repository.GetNextFiveDaysItems(startDate);

            Assert.Equal(2, result.Count);
            Assert.Equal("Test 1", result[0].Description);
            Assert.Equal("Test 2", result[1].Description);
        }
    }
}