using TodoItems.Core.Model;

namespace TodoItems.Core.service;

public interface ITodoItemsRepository
{
    public Task<TodoItem> FindById(string id);
    Task<TodoItem> SaveAsync(TodoItem todoItem);
    public Task<List<TodoItem>> getAllItemsCountInToday(DateTime today);
    Task<List<TodoItem>> getNextFiveDaysItem(DateTime today);
}