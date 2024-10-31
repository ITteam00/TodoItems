using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using TodoItems.Core;
using TodoItems.Core.Service;

namespace TodoItems.Tests
{
    public class UserTests
    {
        [Fact]
        public void AddOneItem_ShouldThrowException_WhenDueDateLimitReached()
        {
            // Arrange
            var mockRepo = new Mock<ITodosRepository>();
            User user = new User(mockRepo.Object);
            var dueDate = new DateTimeOffset(2024, 10, 31, 0, 0, 0, TimeSpan.Zero);
            var item = new TodoItem { Id = "1", Description = "Task 1", DueDate = dueDate };

            mockRepo.Setup(repo => repo.GetItemsByDueDate(dueDate)).Returns(new List<TodoItem>
            {
                new TodoItem(), new TodoItem(), new TodoItem(), new TodoItem(),
                new TodoItem(), new TodoItem(), new TodoItem(), new TodoItem(), new TodoItem()
            });

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => user.AddOneItem(item));
            Assert.Equal("Item due date limit reached for today.", exception.Message);
        }

        [Fact]
        public void AddOneItem_ShouldAddItem_WhenDueDateLimitNotReached()
        {
            // Arrange
            var mockRepo = new Mock<ITodosRepository>();
            var user = new User(mockRepo.Object);
            var dueDate = new DateTimeOffset(2024, 10, 31, 0, 0, 0, TimeSpan.Zero);
            var item = new TodoItem { Id = "1", Description = "Task 1", DueDate = dueDate };

            mockRepo.Setup(repo => repo.GetItemsByDueDate(dueDate)).Returns(new List<TodoItem>());
            mockRepo.Setup(repo => repo.AddItem(item)).Returns(1);

            // Act
            var result = user.AddOneItem(item);

            // Assert
            Assert.Equal(1, result);
            mockRepo.Verify(repo => repo.AddItem(item), Times.Once);
        }

        [Fact]
        public void AddOneItem_ShouldAddItem_WhenDueDateIsNull()
        {
            // Arrange
            var mockRepo = new Mock<ITodosRepository>();
            var user = new User(mockRepo.Object);
            var item = new TodoItem { Id = "1", Description = "Task 1", DueDate = null };

            mockRepo.Setup(repo => repo.AddItem(item)).Returns(1);

            // Act
            var result = user.AddOneItem(item);

            // Assert
            Assert.Equal(1, result);
            mockRepo.Verify(repo => repo.AddItem(item), Times.Once);
        }
    }
}
