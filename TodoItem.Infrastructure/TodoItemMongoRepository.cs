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

    public async Task<UpdateResult> Save(ToDoItemObj todoItem)
    {
        var filter = Builders<TodoItemDao>.Filter.Eq(x => x.Id, todoItem.Id);
        var update = Builders<TodoItemDao>.Update
            .Set(x => x.Description, todoItem.Description)
            .Set(x => x.Done, todoItem.Done)
            .Set(x => x.Favorite, todoItem.Favorite)
            .Set(x => x.CreatedTimeDate, todoItem.CreatedTimeDate)
            .Set(x => x.LastModifiedTimeDate, todoItem.LastModifiedTimeDate)
            .Set(x => x.EditTimes, todoItem.EditTimes)
            .Set(x => x.DueDate, todoItem.DueDate);

        return await _todosCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public List<ToDoItemObj> findAllTodoItemsInOneday(DateTime dateTime)
    {
        var filter = Builders<TodoItemDao>.Filter.Gte(x => x.DueDate, dateTime.Date) &
                     Builders<TodoItemDao>.Filter.Lt(x => x.DueDate, dateTime.Date.AddDays(1));
        var todoItemsDao = _todosCollection.Find(filter).ToList();

        return todoItemsDao.Select(ConvertToTodoItem).ToList();
    }

}