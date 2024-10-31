using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodosRepository
    {
        List<ToDoItemModel> findAllTodoItemsInToday();
    }
}
