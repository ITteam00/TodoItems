using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodoItemsRepository
    {
        List<ToDoItemObj> findAllTodoItemsInToday();
        public Task<ToDoItemObj> FindById(string id);
        public Task<ToDoItemObj> Save(ToDoItemObj todoItem);
    }
}
