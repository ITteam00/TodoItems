using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public class TodosRepository : ITodosRepository
    {
        public List<TodoItem> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            throw new NotImplementedException();
        }

        public int AddItem(TodoItem item)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceAsync(string s, ToDoItemMongoDTO item)
        {
            throw new NotImplementedException();
        }
    }
}