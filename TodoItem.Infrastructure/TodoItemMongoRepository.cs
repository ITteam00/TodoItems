using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;
using TodoItems.Core;
using ToDoItem.Api.Models;

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

    public async Task<ToDoItemObj> FindById(string? id)
    {
        FilterDefinition<TodoItemPo?> filter = Builders<TodoItemPo>.Filter.Eq(x => x.Id, id);
        TodoItemPo? todoItemPo = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        // 将 TodoItemPo 转换为 TodoItem
        ToDoItemObj todoItem = ConvertToTodoItem(todoItemPo);
        return todoItem;
    }

    private ToDoItemObj ConvertToTodoItem(TodoItemPo? todoItemPo)
    {
        if (todoItemPo == null) return null;

        return new ToDoItemObj(
            todoItemPo.Id,
            todoItemPo.Description,
            todoItemPo.Done,
            todoItemPo.Favorite,
            todoItemPo.CreatedTimeDate,
            todoItemPo.LastModifiedTimeDate,
            todoItemPo.EditTimes,
            todoItemPo.DueDate
        );
    }

    public void Save(ToDoItemObj todoItem)
    {
        throw new NotImplementedException();
    }

    public List<ToDoItemObj> findAllTodoItemsInToday()
    {
        return new List<ToDoItemObj> { };
    }

}