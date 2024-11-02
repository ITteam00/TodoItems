namespace TodoItems.Core;

public interface ITodoItemsRepository
{
    public Task<TodoItem> FindById(string id);
    public Task<int> GetAllTodoItemsCountInDueDate(DateTime DueDate);
    public Task<List<TodoItem>> GetAllTodoItemsInFiveDays(DateTime CreatedDate);
    void Save(TodoItem todoItem);
}