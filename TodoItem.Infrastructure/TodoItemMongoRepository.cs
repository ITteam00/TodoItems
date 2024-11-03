using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoItems.Core;
using TodoItems.Core.Service;


namespace TodoItems.Infrastructure
{
    public class TodoItemMongoRepository : ITodosRepository
    {
        private TodoStoreDatabaseSettings Settings;
        public readonly IMongoCollection<TodoItemDto> TodosCollection;

        public TodoItemMongoRepository(IOptions<TodoStoreDatabaseSettings> todoStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(todoStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(todoStoreDatabaseSettings.Value.DatabaseName);
            TodosCollection = mongoDatabase.GetCollection<TodoItemDto>(todoStoreDatabaseSettings.Value.TodoItemsCollectionName);
        }

        public async Task<TodoItems.Core.TodoItem> FindById(string? id)
        {
            FilterDefinition<TodoItemDto?> filter = Builders<TodoItemDto>.Filter.Eq(x => x.Id, id);
            TodoItemDto? todoItemPo = await TodosCollection.Find(filter).FirstOrDefaultAsync();

            // 将 TodoItemPo 转换为 TodoItem
            TodoItems.Core.TodoItem todoItem = ConvertToTodoItem(todoItemPo);
            return todoItem;
        }

        private TodoItems.Core.TodoItem ConvertToTodoItem(TodoItemDto? todoItemPo)
        {
            if (todoItemPo == null) return null;

            return new TodoItems.Core.TodoItem
            {
                Id = todoItemPo.Id,
                Description = todoItemPo.Description
            };
        }
        public List<TodoItem> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            throw new NotImplementedException();
        }

        public TodoItem AddItem(TodoItem item)
        {
            var todoItemDto = TodoItemDto.MapToTodoItemDto(item);
            TodosCollection.InsertOne(todoItemDto);
            return item;
        }

        public List<TodoItem> GetFiveDayItems()
        {
            throw new NotImplementedException();
        }

        public TodoItem GetItemById(string id)
        {
            throw new NotImplementedException();
        }

        public TodoItem UpdateItem(TodoItem item)
        {
            throw new NotImplementedException();
        }
    }
}
