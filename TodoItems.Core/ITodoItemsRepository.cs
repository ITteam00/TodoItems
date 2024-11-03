using MongoDB.Driver;
using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodoItemsRepository
    {

        public Task<List<ToDoItemObj>> FindAllTodoItemsInOneDay(DateTime dateTime);
        public Task<ToDoItemObj> FindById(string id);
        public Task<UpdateResult> Save(ToDoItemObj todoItem);
        public Task<ToDoItemObj> CreateAsync(ToDoItemObj toDoItem);
        public Task<ToDoItemObj> EditItem(ToDoItemObj item);
    }
}
