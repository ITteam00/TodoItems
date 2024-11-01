using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core;
using TodoItems.Core.Service;
namespace TodoItems.Core.Service
{
    public class TodosRepository : ITodosRepository
    {
        static public List<TodoItem> itemsCollection = new List<TodoItem>();

        public TodoItem AddItem(TodoItem item)
        {
            itemsCollection.Add(item);
            return item;
        }

        public List<TodoItem> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            var result = itemsCollection.Where(item =>
            {
                return item.DueDate == dueDate;
            }).ToList();
            return result;
        }

    }
}
