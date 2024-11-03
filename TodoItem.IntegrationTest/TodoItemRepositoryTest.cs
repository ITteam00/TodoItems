using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TodoItem.Infrastructure;
using TodoItems.Core;
using TodoItems.Core.Model;
using TodoItems.Core.Services;

namespace TodoItem.IntegrationTest
{
    public class TodoItemRepositoryTest : IAsyncLifetime
    {
        private readonly ILogger<ToDoItemsService> _mockLogger;
        private readonly TodoItemsRepository _mongoRepository;

        public TodoItemRepositoryTest()
        {
            var mockSettings = new Mock<IOptions<ToDoItemDatabaseSettings>>();

            mockSettings.Setup(s => s.Value).Returns(new ToDoItemDatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27081",
                DatabaseName = "TestTodoDatabase",
                CollectionName = "TodoItems"
            });

            _mongoRepository = new TodoItemsRepository(mockSettings.Object, _mockLogger);
        }

        public async Task InitializeAsync()
        {
            await _mongoRepository.ToDoItemsCollection.DeleteManyAsync(FilterDefinition<TodoItemMongoDTO>.Empty);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async void should_return_item_by_id_1()
        {
            var todoItemPo = new TodoItemMongoDTO()
            {
                Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
                Description = "Buy groceries",
                ModificationDateTimes = new List<DateTimeOffset>(),
                IsDone = false,
                IsFavorite = false,
                DueDate = DateTimeOffset.Now.AddDays(2),
            };
            await _mongoRepository.ToDoItemsCollection.InsertOneAsync(todoItemPo);
            var filter = Builders<TodoItemMongoDTO>.Filter
                .Eq(item => item.Id, "5f9a7d8e2d3b4a1eb8a7d8e2");

            var todoItem = _mongoRepository.ToDoItemsCollection.Find(filter).FirstOrDefault();
            Assert.NotNull(todoItem);
            Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", todoItem.Id);
            Assert.Equal("Buy groceries", todoItem.Description);
            Assert.Equal(todoItemPo.DueDate, todoItem.DueDate);
        }

        [Fact]
        public async Task GetItemsByDueDate_ShouldReturnItemsWithMatchingDueDate()
        {
            // Arrange
            var dueDate = DateTimeOffset.Now;
            var todoItemPo1 = new TodoItemMongoDTO
            {
                Id = "1",
                Description = "Test 1",
                IsDone = false,
                IsFavorite = false,
                DueDate = dueDate,
                ModificationDateTimes = new List<DateTimeOffset>(),
                CreatedTime = DateTimeOffset.Now
            };
            var todoItemPo2 = new TodoItemMongoDTO
            {
                Id = "2",
                Description = "Test 2",
                IsDone = false,
                IsFavorite = false,
                DueDate = dueDate,
                ModificationDateTimes = new List<DateTimeOffset>(),
                CreatedTime = DateTimeOffset.Now
            };

            await _mongoRepository.ToDoItemsCollection.InsertOneAsync(todoItemPo1);
            await _mongoRepository.ToDoItemsCollection.InsertOneAsync(todoItemPo2);

            // Act
            var result = await _mongoRepository.GetItemsByDueDate(dueDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.Equal(dueDate, item.DueDate));
        }

    }
}