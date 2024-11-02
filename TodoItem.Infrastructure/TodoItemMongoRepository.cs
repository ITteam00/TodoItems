using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using TodoItems.Core;

namespace TodoItem.Infrastructure;

public class TodoItemMongoRepository : ITodoItemsRepository
{
    private readonly IMongoCollection<TodoItemPo?> _todosCollection;

    public TodoItemMongoRepository(IOptions<TodoStoreDatabaseSettings> todoStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(todoStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(todoStoreDatabaseSettings.Value.DatabaseName);
        _todosCollection = mongoDatabase.GetCollection<TodoItemPo>(todoStoreDatabaseSettings.Value.TodoItemsCollectionName);
    }

    public async Task<TodoItems.Core.TodoItem> FindById(string? id)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.Id, id);
        TodoItemPo? todoItemPo = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        // 将 TodoItemPo 转换为 TodoItem
        TodoItems.Core.TodoItem todoItem = ConvertToTodoItem(todoItemPo);
        return todoItem;
    }

    public async Task<int> GetAllTodoItemsCountInDueDate(DateTime dueDate)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.DueDate, dueDate);
        List<TodoItemPo?> todoItemPos = await _todosCollection.Find(filter).ToListAsync();
        return todoItemPos.Count;
    }

    public async Task<List<TodoItems.Core.TodoItem>> GetAllTodoItemsInFiveDays(DateTime createdDate)
    {
        DateTime startDate = createdDate;
        DateTime endDate = createdDate.AddDays(5);

        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo?>.Filter.And(
        Builders<TodoItemPo?>.Filter.Gte(x => x.DueDate, startDate),
        Builders<TodoItemPo?>.Filter.Lte(x => x.DueDate, endDate));

        List<TodoItemPo?> todoItemPos = await _todosCollection.Find(filter).ToListAsync();

        return todoItemPos.Select(ConvertToTodoItem).ToList();

    }


    private TodoItems.Core.TodoItem ConvertToTodoItem(TodoItemPo? todoItemPo)
    {
        if (todoItemPo == null) return null;

        return new TodoItems.Core.TodoItem
        {
            Id = todoItemPo.Id,
            Description = todoItemPo.Description,
            DueDate = todoItemPo.DueDate,
        };
    }

    public void Save(TodoItems.Core.TodoItem todoItem)
    {
        throw new NotImplementedException();
    }
}
