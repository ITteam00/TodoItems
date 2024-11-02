using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using TodoItems.Core;
using TodoItems.Core.Service;
using Xunit.Abstractions;

namespace TodoItems.Tests
{
    public class TodoItemServiceTests
    {
        [Fact]
        public void Create_ShouldAddItem_WhenValid()
        {
            // Arrange
            var mockRepo = new Mock<ITodosRepository>();
            var service = new TodoItemService(mockRepo.Object);
            var id = "1";
            var description = "Test item";

            // Act
            var result = service.Create(id, description);

            // Assert
            mockRepo.Verify(r => r.AddItem(It.Is<TodoItem>(i => i.Id == id && i.Description == description)), Times.Once);
        }

        [Fact]
        public void Create_ShouldThrowException_WhenDueDateIsEarlierThanToday()
        {
            // Arrange
            var mockRepo = new Mock<ITodosRepository>();
            var service = new TodoItemService(mockRepo.Object);
            var id = "1";
            var description = "Test item";
            var dueDate = DateTimeOffset.Now.AddDays(-1).Date;
            var item = new TodoItem { Id = id, Description = description, DueDate =  dueDate};
            mockRepo.Setup(r => r.AddItem(item)).Returns(item);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => service.Create(id, description, dueDate));
            Assert.Equal("Item due date is earlier than today.", exception.Message);
        }

        [Fact]
        public void Create_ShouldThrowException_WhenDueDateLimitReached()
        {
            // Arrange
            var mockRepo = new Mock<ITodosRepository>();
            var service = new TodoItemService(mockRepo.Object);
            var id = "1";
            var description = "Test item";
            var dueDate = DateTimeOffset.Now.Date;
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate },
                new TodoItem { DueDate = dueDate }
            };
            mockRepo.Setup(r => r.GetItemsByDueDate(dueDate)).Returns(items);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => service.Create(id, description, dueDate));
            Assert.Equal("Item due date limit reached for today.", exception.Message);
        }

        [Fact]
        public void ModifyItem_ShouldReturnModifiedItem_WhenModifiedLimitNotReached()
        {
            var mockRepo = new Mock<ITodosRepository>();
            var service = new TodoItemService(mockRepo.Object);

            string id = "1";
            string newDescription = "Test item";
            var modification = new Modification();
            var item = new TodoItem
            {
                Id = id,
                Description = "old description",
                ModificationRecord = modification,
            };

            mockRepo.Setup(r => r.GetItemById(id)).Returns(
                new TodoItem
                {
                    Id = id, 
                    Description = newDescription,
                    ModificationRecord = modification,

                });
            var result = service.ModifyItem(id, newDescription);
            Assert.Equal(newDescription, result.Description);
        }
    }
}
