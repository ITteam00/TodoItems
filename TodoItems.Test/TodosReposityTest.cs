using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TodoItems.Core;
using TodoItems.Core.Service;

namespace TodoItems.Test
{
    public class TodosRepositoryTest
    {
        [Fact]
        public void GetItemsByDueDate_ShouldReturnCorrectItems()
        {
            // Arrange
            var repository = new TodosRepository();
            var dueDate = new DateTimeOffset(2024, 10, 31, 0, 0, 0, TimeSpan.Zero);
            var items = new List<TodoItem>
            {
                new TodoItem { Id = "1", Description = "Task 1", DueDate = dueDate },
                new TodoItem { Id = "2", Description = "Task 2", DueDate = new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero) },
                new TodoItem { Id = "3", Description = "Task 3", DueDate = dueDate }
            };
            TodosRepository.itemsCollection = items;

            //var field = typeof(TodosRepository).GetField("_itemsCollection", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            //field.SetValue(null, items);

            // Act
            var result = repository.GetItemsByDueDate(dueDate);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, item => item.Id == "1");
            Assert.Contains(result, item => item.Id == "3");
        }
    }
}
