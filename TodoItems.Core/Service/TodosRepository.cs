using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core;

namespace TodoItems.Core.Service
{
    public class TodosRepository
    {
        static public List<TodoItem> itemsCollection = new List<TodoItem>();
        public List<TodoItem> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            var result =  itemsCollection.Where(item =>
            {
                return item.DueDate == dueDate;
            }).ToList();
            return result;
        }
    }
}
