using TodoItems.Core.Model;

namespace TodoItems.Core.service;

public interface ITodoItemsRepository
{
    public Task<TodoItem> FindById(string id);
    Task<TodoItem> CreateTodoItemAsync(TodoItem todoItem);
    public Task<List<TodoItem>> GetAllItemsInDueDate(DateTime today);
    Task<List<TodoItem>> GetNextFiveDaysItem(DateTime today);
    public Task UpdateAsync(string id,TodoItem updateItem);

}