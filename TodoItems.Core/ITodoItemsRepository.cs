using MongoDB.Driver;
using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodoItemsRepository
    {

        public List<ToDoItemObj> findAllTodoItemsInOneday(DateTime dateTime);
        public Task<ToDoItemObj> FindById(string id);
        public Task<UpdateResult> Save(ToDoItemObj todoItem);
        public Task<ToDoItemObj> CreateAsync(ToDoItemObj toDoItem);
        public Task<ToDoItemObj> ModifyItem(ToDoItemObj item);
    }
}
