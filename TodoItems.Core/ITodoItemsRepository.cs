using MongoDB.Driver;
using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodoItemsRepository
    {
        List<ToDoItemObj> findAllTodoItemsInToday();
        public Task<ToDoItemObj> FindById(string id);
        public Task<UpdateResult> Save(ToDoItemObj todoItem);
    }
}
