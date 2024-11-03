using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoItems.Core.Model;
using TodoItems.Core.service;

namespace TodoItem.Infrastructure;

public class TodoItemMongoRepository: ITodoItemsRepository
{
    private readonly IMongoCollection<TodoItemPo?> _todosCollection;
    
    public TodoItemMongoRepository(IOptions<TodoStoreDatabaseSettings> todoStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(todoStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(todoStoreDatabaseSettings.Value.DatabaseName);
        _todosCollection = mongoDatabase.GetCollection<TodoItemPo>(todoStoreDatabaseSettings.Value.TodoItemsCollectionName);
    }

    public async Task<TodoItems.Core.Model.TodoItem> FindById(string? id)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.Id, id);
        TodoItemPo? todoItemPo = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        // 将 TodoItemPo 转换为 TodoItem
        TodoItems.Core.Model.TodoItem todoItem = ConvertToTodoItem(todoItemPo);
        return todoItem;
    }

    private TodoItems.Core.Model.TodoItem ConvertToTodoItem(TodoItemPo? todoItemPo)
    {
        if (todoItemPo == null) return null;

        return new TodoItems.Core.Model.TodoItem
        {
            Id = todoItemPo.Id,
            Description = todoItemPo.Description,
            DueDate=todoItemPo.DueDate.ToLocalTime(),
            IsComplete = todoItemPo.IsComplete,
        };
    }

    private TodoItemPo ConvertToTodoItemPo(TodoItems.Core.Model.TodoItem todoItem)
    {
        if (todoItem == null) return null;

        return new TodoItemPo
        {
            Id = todoItem.Id,
            Description = todoItem.Description,
            DueDate = todoItem.DueDate.ToLocalTime(),
            IsComplete = todoItem.IsComplete,
        };
    }

    public async Task<TodoItems.Core.Model.TodoItem> CreateTodoItemAsync(TodoItems.Core.Model.TodoItem todoItem)
    {
        await _todosCollection.InsertOneAsync(ConvertToTodoItemPo(todoItem));
        return todoItem;
    }

    public async Task<List<TodoItems.Core.Model.TodoItem>> GetAllItemsInDueDate(DateTime dueDate)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.DueDate, dueDate);
        List<TodoItemPo>? todoItemPos = await _todosCollection.Find(filter).ToListAsync();
        List<TodoItems.Core.Model.TodoItem> todoItems = [];
        foreach (TodoItemPo item in todoItemPos)
        {
            todoItems.Add(ConvertToTodoItem(item));
        }
        return todoItems;
    }

    public async Task<List<TodoItems.Core.Model.TodoItem>> GetNextFiveDaysItem(DateTime date)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.And(
            Builders<TodoItemPo>.Filter.Gte(item => item.DueDate, date),
            Builders<TodoItemPo>.Filter.Lt(item => item.DueDate, date.AddDays(5)));
        var nextFiveDaysItem = await _todosCollection.Find(filter).ToListAsync();
        return nextFiveDaysItem.Select(item => ConvertToTodoItem(item)).ToList();
    }

    public async Task UpdateAsync(string id, TodoItems.Core.Model.TodoItem updateTodoItem)
    {
        TodoItemPo updateItem=ConvertToTodoItemPo(updateTodoItem);
        await _todosCollection.ReplaceOneAsync(x=>x.Id==id,updateItem);
    }

}
