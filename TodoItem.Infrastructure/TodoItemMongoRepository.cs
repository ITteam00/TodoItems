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

    public async Task<TodoItems.Core.TodoItemDto> FindById(string? id)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.Id, id);
        TodoItemPo? todoItemPo = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        TodoItems.Core.TodoItemDto todoItem = ConvertToTodoItem(todoItemPo);
        return todoItem;
    }

    public async Task<int> GetAllTodoItemsCountInDueDate(DateTime? dueDate)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.DueDate, dueDate);
        List<TodoItemPo?> todoItemPos = await _todosCollection.Find(filter).ToListAsync();
        return todoItemPos.Count;
    }

    public async Task<List<TodoItems.Core.TodoItemDto>> GetAllTodoItemsInFiveDays(DateTime createdDate)
    {
        DateTime startDate = createdDate;
        DateTime endDate = createdDate.AddDays(5);

        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo?>.Filter.And(
        Builders<TodoItemPo?>.Filter.Gte(x => x.DueDate, startDate),
        Builders<TodoItemPo?>.Filter.Lte(x => x.DueDate, endDate));

        List<TodoItemPo?> todoItemPos = await _todosCollection.Find(filter).ToListAsync();

        return todoItemPos.Select(ConvertToTodoItem).ToList();

    }


    private TodoItems.Core.TodoItemDto ConvertToTodoItem(TodoItemPo? todoItemPo)
    {
        if (todoItemPo == null) return null;

        return new TodoItems.Core.TodoItemDto
        {
            Id = todoItemPo.Id,
            Description = todoItemPo.Description,
            DueDate = todoItemPo.DueDate,
        };
    }

    private TodoItemPo ReconvertToTodoItem(TodoItems.Core.TodoItemDto todoItemDto)
    {
        if (todoItemDto == null) return null;

        return new TodoItemPo
        {
            Id = todoItemDto.Id,
            Description = todoItemDto.Description,
            DueDate = (DateTime)todoItemDto.DueDate,
        };
    }

    public async Task Save(TodoItems.Core.TodoItemDto todoItem)
    {
        TodoItemPo todoItemPo = ReconvertToTodoItem(todoItem);
        await _todosCollection.InsertOneAsync(todoItemPo);
    }
}
