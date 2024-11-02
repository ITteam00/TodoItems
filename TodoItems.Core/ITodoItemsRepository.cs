namespace TodoItems.Core;

public interface ITodoItemsRepository
{
    public Task<TodoItem> FindById(string id);
    void Save(TodoItem todoItem);
    public Task<List<TodoItem>> getAllItemsCountInToday(DateTime today);
}