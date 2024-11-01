
namespace TodoItems.Core.Service
{
    public interface ITodosRepository
    {
        List<TodoItem> GetItemsByDueDate(DateTimeOffset dueDate);
        TodoItem AddItem(TodoItem item);
    }
}