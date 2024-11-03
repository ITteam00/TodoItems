using System;
using System.Collections.Generic;
using Xunit;
using TodoItems.Core;

namespace TodoItems.Test
{
    public class DueDateStrategyTest
    {
        [Fact]
        public void GetEarliestDate_ShouldReturnEarliestDate_WhenItemsAreValid()
        {
            // Arrange
            var strategy = new GetEarliestDateStategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 3)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) }
            };
            int completedLimit = 5;

            // Act
            var result = strategy.GetDate(items, completedLimit);

            // Assert
            Assert.Equal(new DateTimeOffset(new DateTime(2024, 1, 1)), result);
        }

        [Fact]
        public void GetEarliestDate_ShouldThrowException_WhenNoDateMeetsLimit()
        {
            // Arrange
            var strategy = new GetEarliestDateStategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) }
            };
            int completedLimit = 2;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => strategy.GetDate(items, completedLimit));
        }

        [Fact]
        public void GetFewestCompleted_ShouldReturnDateWithFewestItems_WhenItemsAreValid()
        {
            // Arrange
            var strategy = new GetFewestCompletedStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 3)) }
            };
            int completedLimit = 5;

            // Act
            var result = strategy.GetDate(items, completedLimit);

            // Assert
            Assert.Equal(new DateTimeOffset(new DateTime(2024, 1, 1)), result);
        }

        [Fact]
        public void GetFewestCompleted_ShouldThrowException_WhenNoDateMeetsLimit()
        {
            // Arrange
            var strategy = new GetFewestCompletedStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) }
            };
            int completedLimit = 2;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => strategy.GetDate(items, completedLimit));
        }

        [Fact]
        public void GetEarliestDate_ShouldReturnCurrentDate_WhenItemsAreNullOrEmpty()
        {
            // Arrange
            var strategy = new GetEarliestDateStategy();
            List<TodoItem> items = null;
            int completedLimit = 5;

            // Act
            var result = strategy.GetDate(items, completedLimit);

            // Assert
            Assert.Equal(DateTimeOffset.Now.Date, result.Date);
        }

        [Fact]
        public void GetFewestCompleted_ShouldReturnCurrentDate_WhenItemsAreNullOrEmpty()
        {
            // Arrange
            var strategy = new GetFewestCompletedStrategy();
            List<TodoItem> items = null;
            int completedLimit = 5;

            // Act
            var result = strategy.GetDate(items, completedLimit);

            // Assert
            Assert.Equal(DateTimeOffset.Now.Date, result.Date);
        }

        [Fact]
        public void GetFewestCompleted_ShouldReturnDateWithFewestItems_WhenItemsWithNull()
        {
            // Arrange
            var strategy = new GetFewestCompletedStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { }
            };
            int completedLimit = 5;

            // Act
            var result = strategy.GetDate(items, completedLimit);

            // Assert
            Assert.Equal(new DateTimeOffset(new DateTime(2024, 1, 1)), result);
        }
    }
}
