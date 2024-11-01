namespace TodoItems.Core
{
    public interface ITodoItemService
    {
        TodoItemDto CreateTodoItem(TodoItemDto item);
        TodoItemDto ModifyTodoItem(TodoItemDto oldItem, TodoItemDto newItem);
    }
}