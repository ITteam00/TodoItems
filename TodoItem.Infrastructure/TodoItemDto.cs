using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core;

namespace TodoItems.Infrastructure
{
    public class TodoItemDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // 确保 Id 被存储为字符串
        public string Id { get; set; }
        public string Description { get; set; }
        public ModificationDto ModificationRecord { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public bool Done { get; set; } = false;
        public DateTimeOffset CreatedTime { get; set; }
        
        public static TodoItemDto MapToTodoItemDto(TodoItem todoItem)
        {
            var dto = new TodoItemDto
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
                ModificationRecord = new ModificationDto()
                {
                    ModifiedTimes = new List<DateTimeOffset>(todoItem.ModificationRecord.ModifiedTimes),
                },
                DueDate = todoItem.DueDate,
                Done = todoItem.Done,
                CreatedTime = todoItem.CreatedTime,
            };
            return dto;

        }


        public TodoItem MapToTodoItem()
        {
            var item = new TodoItem
            {
                Id = this.Id,
                Description = this.Description,
                ModificationRecord = new Modification()
                {
                    ModifiedTimes = new List<DateTimeOffset>(this.ModificationRecord.ModifiedTimes),
                },
                DueDate = this.DueDate,
                Done = this.Done,
                CreatedTime = this.CreatedTime,
            };
            
            return item;
        }
    }
}
