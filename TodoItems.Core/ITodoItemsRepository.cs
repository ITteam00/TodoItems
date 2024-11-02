namespace TodoItems.Core;

public interface ITodoItemsRepository
{
    public Task<TodoItem> FindById(string id);
    public Task<int> GetAllTodoItemsCountInDueDate(DateTime DueDate);

    void Save(TodoItem todoItem);
}