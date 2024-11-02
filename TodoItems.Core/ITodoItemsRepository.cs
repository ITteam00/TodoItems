using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodoItemsRepository
    {
        List<ToDoItemObj> findAllTodoItemsInToday();
        public Task<ToDoItemObj> FindById(string id);
        void Save(ToDoItemObj todoItem);
    }
}
