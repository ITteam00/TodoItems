
namespace TodoItems.Core.Service
{
    public interface ITodosRepository
    {
        List<TodoItem> GetItemsByDueDate(DateTimeOffset dueDate);
        int AddItem(TodoItem item);
    }
}