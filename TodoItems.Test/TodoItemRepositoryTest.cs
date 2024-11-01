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
        private readonly Mock<ILogger<ToDoItemsService>> _mockLogger;
        private readonly Mock<IOptions<ToDoItemDatabaseSettings>> _mockSettings;
        private readonly TodoItemsRepository _repository;
        private readonly Mock<IFindFluent<TodoItemMongoDTO, TodoItemMongoDTO>> _mockFindFluent;
        private readonly Mock<IAsyncCursor<TodoItemMongoDTO>> _mockCursor;

        public TodoItemRepositoryTest()
        {
            _mockFindFluent = new Mock<IFindFluent<TodoItemMongoDTO, TodoItemMongoDTO>>();
            _mockCollection = new Mock<IMongoCollection<TodoItemMongoDTO>>();
            _mockLogger = new Mock<ILogger<ToDoItemsService>>();
            _mockSettings = new Mock<IOptions<ToDoItemDatabaseSettings>>();
            _mockCursor = new Mock<IAsyncCursor<TodoItemMongoDTO>>();

            var settings = new ToDoItemDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "TodoDatabase",
                CollectionName = "TodoItems"
            };
            _mockSettings.Setup(s => s.Value).Returns(settings);

            _repository = new TodoItemsRepository(_mockSettings.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReplaceOneAsync()
        {
            // Arrange
            var id = "testId";
            var updatedTodoItem = new TodoItemDTO()
            {
                Id = "1",
                Description = "Updated Description",
                IsDone = true,
                IsFavorite = false,
                CreatedTime = DateTime.UtcNow
            };

            var filterDefinition = Builders<TodoItemMongoDTO>.Filter.Eq(x => x.Id, id);
            _mockCollection.Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TodoItemMongoDTO>>(),
                    It.IsAny<TodoItemMongoDTO>(), It.IsAny<ReplaceOptions>(), default))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, id));

            await _repository.UpdateAsync(id, updatedTodoItem);

            _mockCollection.Verify(
                c => c.ReplaceOneAsync(It.Is<FilterDefinition<TodoItemMongoDTO>>(f => f == filterDefinition),
                    It.IsAny<TodoItemMongoDTO>(), It.IsAny<ReplaceOptions>(), default), Times.Once);
        }

        [Fact]
        public async Task GetItemsByDueDate_ReturnsTodoItems()
        {
            // Arrange
            var dueDate = DateTimeOffset.Now;
            var todoItems = new List<TodoItemMongoDTO>
            {
                new TodoItemMongoDTO
                {
                    Id = "1", Description = "Test 1", isDone = false, isFavorite = true,
                    CreatedTime = DateTimeOffset.Now, DueDate = dueDate
                },
                new TodoItemMongoDTO
                {
                    Id = "2", Description = "Test 2", isDone = true, isFavorite = false,
                    CreatedTime = DateTimeOffset.Now, DueDate = dueDate
                }
            };

            _mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            _mockCursor.Setup(x => x.Current).Returns(todoItems);

            _mockFindFluent.Setup(m => m.ToCursor(It.IsAny<CancellationToken>())).Returns(_mockCursor.Object);
            _mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<TodoItemMongoDTO>>(), null))
                .Returns(_mockFindFluent.Object);

            // Act
            var result = await _repository.GetItemsByDueDate(dueDate);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test 1", result[0].Description);
            Assert.Equal("Test 2", result[1].Description);
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
                isDone = newTodoItem.IsDone,
                isFavorite = newTodoItem.IsFavorite,
                CreatedTime = newTodoItem.CreatedTime,
            };

            // Act
            await _repository.CreateAsync(newTodoItem);

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(It.Is<TodoItemMongoDTO>(i => i.Id == item.Id &&
                        i.Description == item.Description &&
                        i.isDone == item.isDone &&
                        i.isFavorite == item.isFavorite &&
                        i.CreatedTime == item.CreatedTime),
                    null,
                    default),
                Times.Once);

            _mockLogger.Verify(l => l.LogInformation($"Inserting new todo item to DB {newTodoItem.Id}"), Times.Once);
        }
    }
}