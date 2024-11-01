using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodosRepository
    {
        List<ToDoItem.Api.Models.ToDoItemObj> findAllTodoItemsInToday();
    }
}
