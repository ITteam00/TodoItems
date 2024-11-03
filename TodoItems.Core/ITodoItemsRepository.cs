namespace TodoItems.Core;

public interface ITodoItemsRepository
{
    public Task<TodoItem> FindById(string id);
    Task<TodoItem> SaveAsync(TodoItem todoItem);
    public Task<List<TodoItem>> getAllItemsCountInToday(DateTime today);
}