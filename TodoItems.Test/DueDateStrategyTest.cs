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
            var strategy = new DueDateStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 3)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) }
            };
            int completedLimit = 5;

            // Act
            var result = strategy.GetEarliestDate(items, completedLimit);

            // Assert
            Assert.Equal(new DateTimeOffset(new DateTime(2024, 1, 1)), result);
        }

        [Fact]
        public void GetEarliestDate_ShouldThrowException_WhenNoDateMeetsLimit()
        {
            // Arrange
            var strategy = new DueDateStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) }
            };
            int completedLimit = 2;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => strategy.GetEarliestDate(items, completedLimit));
        }

        [Fact]
        public void GetFewestCompleted_ShouldReturnDateWithFewestItems_WhenItemsAreValid()
        {
            // Arrange
            var strategy = new DueDateStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 3)) }
            };
            int completedLimit = 5;

            // Act
            var result = strategy.GetFewestCompleted(items, completedLimit);

            // Assert
            Assert.Equal(new DateTimeOffset(new DateTime(2024, 1, 1)), result);
        }

        [Fact]
        public void GetFewestCompleted_ShouldThrowException_WhenNoDateMeetsLimit()
        {
            // Arrange
            var strategy = new DueDateStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) }
            };
            int completedLimit = 2;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => strategy.GetFewestCompleted(items, completedLimit));
        }

        [Fact]
        public void GetEarliestDate_ShouldReturnCurrentDate_WhenItemsAreNullOrEmpty()
        {
            // Arrange
            var strategy = new DueDateStrategy();
            List<TodoItem> items = null;
            int completedLimit = 5;

            // Act
            var result = strategy.GetEarliestDate(items, completedLimit);

            // Assert
            Assert.Equal(DateTimeOffset.Now.Date, result.Date);
        }

        [Fact]
        public void GetFewestCompleted_ShouldReturnCurrentDate_WhenItemsAreNullOrEmpty()
        {
            // Arrange
            var strategy = new DueDateStrategy();
            List<TodoItem> items = null;
            int completedLimit = 5;

            // Act
            var result = strategy.GetFewestCompleted(items, completedLimit);

            // Assert
            Assert.Equal(DateTimeOffset.Now.Date, result.Date);
        }

        [Fact]
        public void GetFewestCompleted_ShouldReturnDateWithFewestItems_WhenItemsWithNull()
        {
            // Arrange
            var strategy = new DueDateStrategy();
            var items = new List<TodoItem>
            {
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 1)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { DueDate = new DateTimeOffset(new DateTime(2024, 1, 2)) },
                new TodoItem { }
            };
            int completedLimit = 5;

            // Act
            var result = strategy.GetFewestCompleted(items, completedLimit);

            // Assert
            Assert.Equal(new DateTimeOffset(new DateTime(2024, 1, 1)), result);
        }
    }
}
