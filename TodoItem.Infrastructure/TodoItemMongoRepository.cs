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
    private const int MAX_DUEDATE = 8;


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
            todoItemDao.DueDate,
            DueDateRequirementType.Earliest
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

    public async Task<ToDoItemObj> CreateAsync(ToDoItemObj inputToDoItem)
    {
        if (inputToDoItem.DueDate == null && inputToDoItem.DueDateRequirement == null)
        {
            throw new InvalidOperationException("Due Date and DueDateRequirement cannot be empty at the same time.");
        }

        if (inputToDoItem.DueDate != null)
        {
            inputToDoItem.ValidateDueDate();
            var itemsDueToday = findAllTodoItemsInOneday((DateTime)inputToDoItem.DueDate);
            if (itemsDueToday.Count >= MAX_DUEDATE)
            {
                throw new InvalidOperationException("Cannot add more than 8 ToDo items for today.");
            }

        } else
        {
            DateTime newDueDate = FindGoodDueDateByRequirement((DueDateRequirementType)inputToDoItem.DueDateRequirement);
            inputToDoItem.DueDate = newDueDate;
        }
        await Save(inputToDoItem);
        return await this.FindById(inputToDoItem.Id);

    }

    private DateTime FindGoodDueDateByRequirement(DueDateRequirementType requirement)
    {
        int[] itemNumbers = GetItemNumbersForNext5Days();

        if (itemNumbers.All(count => count == 8))
        {
            throw new InvalidOperationException("Next 5 days all have 8 toDoItems");
        }

        DateTime earliestDate = DateTime.UtcNow.Date;
        for (int i = 0; i < itemNumbers.Length; i++)
        {
            if (itemNumbers[i] < 8)
            {
                earliestDate = DateTime.UtcNow.Date.AddDays(i);
                break;
            }
        }

        int minItems = itemNumbers.Min();
        DateTime fewestDate = DateTime.UtcNow.Date;
        for (int i = 0; i < itemNumbers.Length; i++)
        {
            if (itemNumbers[i] == minItems)
            {
                fewestDate = DateTime.UtcNow.Date.AddDays(i);
                break;
            }
        }

        if (requirement == DueDateRequirementType.Earliest)
        {
            return earliestDate;
        }
        return fewestDate;

    }

    private int[] GetItemNumbersForNext5Days()
    {
        int[] itemNumbers = new int[5];

        for (int i = 0; i < 5; i++)
        {
            DateTime targetDate = DateTime.UtcNow.Date.AddDays(i);
            var itemsOnDate = findAllTodoItemsInOneday(targetDate);
            itemNumbers[i] = itemsOnDate.Count;
        }

        return itemNumbers;
    }

    public async Task<ToDoItemObj> ModifyItem(ToDoItemObj item)
    {
        DateTime lastModifiedDate = item.LastModifiedTimeDate;
        DateTime currentDate = DateTimeOffset.Now.Date;
        TimeSpan difference = currentDate - lastModifiedDate;

        if (difference.Days >= 1)
        {
            item.EditTimes = 1;
        }
        else
        {
            item.IncrementEditTimes();
        }

        item.LastModifiedTimeDate = currentDate;
        await Save(item);
        return item;
    }

}