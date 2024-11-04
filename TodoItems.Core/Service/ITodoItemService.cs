using TodoItems.Core.Model;

namespace TodoItems.Core.service
{
    public interface ITodoItemService
    {
        Task<TodoItem> CreateTodoItem(TodoItem item, string? type = "");
        Task<TodoItem> ModifyTodoItem(string id, TodoItem newItem);
    }
}