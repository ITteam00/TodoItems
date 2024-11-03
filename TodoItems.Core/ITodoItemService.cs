namespace TodoItems.Core
{
    public interface ITodoItemService
    {
        Task<TodoItem> CreateTodoItem(TodoItem item, string? type);
        TodoItem ModifyTodoItem(TodoItem oldItem, TodoItem newItem);
    }
}