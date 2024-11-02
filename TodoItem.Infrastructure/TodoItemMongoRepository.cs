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
        TodoItemDao? todoItemDao = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        // 将 TodoItemDao 转换为 TodoItem
        ToDoItemObj todoItem = ConvertToTodoItem(todoItemDao);
        return todoItem;
    }

    private ToDoItemObj ConvertToTodoItem(TodoItemDao? todoItemDao)
    {
        if (todoItemDao == null) return null;

        return new ToDoItemObj(
            todoItemDao.Id,
            todoItemDao.Description,
            todoItemDao.Done,
            todoItemDao.Favorite,
            todoItemDao.CreatedTimeDate,
            todoItemDao.LastModifiedTimeDate,
            todoItemDao.EditTimes,
            todoItemDao.DueDate
        );
    }

    public Task<ToDoItemObj> Save(ToDoItemObj todoItem)
    {
        throw new NotImplementedException();
    }

    public List<ToDoItemObj> findAllTodoItemsInToday()
    {
        return new List<ToDoItemObj> { };
    }

}