using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Service;

namespace TodoItems.Core
{
    public class TodoItemService
    {
        public TodoItem Create(string id, string description)
        {
            ITodosRepository todosRepository = new TodosRepository();
            var user = new User(todosRepository);
            var item = new TodoItem
            {
                Id = id,
                Description = description,
            };
            user.AddOneItem(item);
            return item;
        }
    }
}
