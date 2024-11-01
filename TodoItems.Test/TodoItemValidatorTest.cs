using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;
using TodoItems.Core.Validator;

namespace TodoItems.Test
{
    public class TodoItemValidatorTest
    {
        private readonly TodoItemValidator _todoItemValidator;

        public TodoItemValidatorTest()
        {
            _todoItemValidator = new TodoItemValidator();
        }

        [Fact]
        public void Should_return_modificationCount_when_call_ModificationCount()
        {
            List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
            {
                new DateTimeOffset(2024, 10, 30, 14, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 9, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 10, 29, 18, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 17, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 16, 2, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 16, 2, 0, TimeSpan.Zero)
            };

            Assert.Equal(4, _todoItemValidator.ModificationCount(dateTimes));
        }


        [Fact]
        public void CountDueDates_ShouldThrowArgumentNullException_WhenTodoItemsIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _todoItemValidator.CountDueDates(null));
        }

        [Fact]
        public void CountDueDates_ShouldReturnCorrectCounts()
        {
            // Arrange
            var todoItems = new List<TodoItemDTO>
            {
                new TodoItemDTO
                {
                    Id = "1", Description = "Task 1", DueDate = new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero)
                },
                new TodoItemDTO
                {
                    Id = "2", Description = "Task 2", DueDate = new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero)
                },
                new TodoItemDTO
                {
                    Id = "3", Description = "Task 3", DueDate = new DateTimeOffset(2024, 11, 2, 0, 0, 0, TimeSpan.Zero)
                },
                new TodoItemDTO { Id = "4", Description = "Task 4", DueDate = null }
            };

            // Act
            var result = _todoItemValidator.CountDueDates(todoItems);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result[new DateTime(2024, 11, 1)]);
            Assert.Equal(1, result[new DateTime(2024, 11, 2)]);
        }


        [Fact]
        public void IsTodady_ShouldReturnTrue_WhenDateIsToday()
        {
            var today = DateTimeOffset.Now;
            var result = _todoItemValidator.IsTodady(today);
            Assert.True(result);
        }

        [Fact]
        public void IsTodady_ShouldReturnFalse_WhenDateIsNotToday()
        {
            var notToday = DateTimeOffset.Now.AddDays(-1);
            var result = _todoItemValidator.IsTodady(notToday);
            Assert.False(result);
        }

        [Fact]
        public void CanModify_ShouldReturnFalse_WhenModificationLimitExceeded()
        {
            // Arrange
            var today = DateTimeOffset.Now.Date;
            var updatedToDoItem = new TodoItemDTO
            {
                Id = "test-id",
                ModificationDateTimes = new List<DateTimeOffset>
                {
                    today.AddHours(1),
                    today.AddHours(2),
                    today.AddHours(3),
                    today.AddHours(1),
                    today.AddHours(1),
                    today.AddHours(2),
                    today.AddHours(3),
                    today.AddHours(1),
                    today.AddHours(2),
                    today.AddHours(3),
                }
            };

            // Act
            var result = _todoItemValidator.CanModify(updatedToDoItem);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanModify_ShouldReturnTrue_WhenModificationLimitNotExceeded()
        {
            var today = DateTimeOffset.Now.Date;
            var updatedToDoItem = new TodoItemDTO
            {
                Id = "test-id",
                ModificationDateTimes = new List<DateTimeOffset>
                {
                    today.AddHours(1),
                    today.AddHours(2)
                }
            };

            // Act
            var result = _todoItemValidator.CanModify(updatedToDoItem);

            // Assert
            Assert.True(result);
        }
    }
}