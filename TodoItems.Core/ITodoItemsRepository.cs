namespace TodoItems.Core;

public interface ITodoItemsRepository
{
    public Task<TodoItemDto> FindById(string id);
    public Task<int> GetAllTodoItemsCountInDueDate(DateTime? dueDate);
    public Task<List<TodoItemDto>> GetAllTodoItemsInFiveDays(DateTime CreatedDate);
    void Save(TodoItemDto todoItem);
}