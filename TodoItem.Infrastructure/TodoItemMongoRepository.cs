using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;
using TodoItems.Core;
using ToDoItem.Api.Models;

namespace TodoItem.Infrastructure;

public class TodoItemMongoRepository : ITodoItemsRepository
{
    private readonly IMongoCollection<TodoItemDao?> _todosCollection;

    public TodoItemMongoRepository(IOptions<TodoStoreDatabaseSettings> todoStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(todoStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(todoStoreDatabaseSettings.Value.DatabaseName);
        _todosCollection = mongoDatabase.GetCollection<TodoItemDao>(todoStoreDatabaseSettings.Value.TodoItemsCollectionName);
    }

    public async Task<ToDoItemObj> FindById(string? id)
    {
        FilterDefinition<TodoItemDao?> filter = Builders<TodoItemDao>.Filter.Eq(x => x.Id, id);
        TodoItemDao? todoItemPo = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        // 将 TodoItemPo 转换为 TodoItem
        ToDoItemObj todoItem = ConvertToTodoItem(todoItemPo);
        return todoItem;
    }

    private ToDoItemObj ConvertToTodoItem(TodoItemDao? todoItemPo)
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