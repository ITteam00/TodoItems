using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public class TodoItemsRepository : ITodosRepository
    {
        private readonly IMongoCollection<TodoItemMongoDTO> _ToDoItemsCollection;
        private readonly ILogger<ToDoItemsService> _Logger;

        public TodoItemsRepository(IOptions<ToDoItemDatabaseSettings> ToDoItemStoreDatabaseSettings,
            ILogger<ToDoItemsService> logger)
        {
            var mongoClient = new MongoClient(
                ToDoItemStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                ToDoItemStoreDatabaseSettings.Value.DatabaseName);

            _ToDoItemsCollection = mongoDatabase.GetCollection<TodoItemMongoDTO>(
                ToDoItemStoreDatabaseSettings.Value.CollectionName);
            _Logger = logger;
        }

        public async Task<List<TodoItemDTO>> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            var filter = Builders<TodoItemMongoDTO>.Filter.Eq(item => item.DueDate, dueDate);
            var todoItems = await _ToDoItemsCollection.Find(filter).ToListAsync();

            return todoItems.Select(item => new TodoItemDTO
            {
                Id = item.Id,
                Description = item.Description,
                IsDone = item.isDone,
                IsFavorite = item.isFavorite,
                CreatedTime = item.CreatedTime,
                DueDate = item.DueDate
            }).ToList();
        }


        public async Task UpdateAsync(string id, TodoItemDTO updatedTodoItem)
        {
            var item = new TodoItemMongoDTO
            {
                Id = id,
                Description = updatedTodoItem.Description,
                isDone = updatedTodoItem.IsDone,
                isFavorite = updatedTodoItem.IsFavorite,
                CreatedTime = updatedTodoItem.CreatedTime,
            };
            await _ToDoItemsCollection.ReplaceOneAsync(x => x.Id == id, item);
        }

        public async Task CreateAsync(TodoItemDTO newTodoItem)
        {
            var item = new TodoItemMongoDTO
            {
                Id = newTodoItem.Id,
                Description = newTodoItem.Description,
                isDone = newTodoItem.IsDone,
                isFavorite = newTodoItem.IsFavorite,
                CreatedTime = newTodoItem.CreatedTime,
            };
            _Logger.LogInformation($"Inserting new todo item to DB {newTodoItem.Id}");

            await _ToDoItemsCollection.InsertOneAsync(item);
            ;
        }

        public List<TodoItemDTO> GetNextFiveDaysItems(DateTimeOffset dueDate)
        {
            throw new NotImplementedException();
        }
    }
}