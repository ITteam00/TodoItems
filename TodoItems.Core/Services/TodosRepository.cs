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
        public static List<ToDoItemDTO> itemsCollection = new List<ToDoItemDTO>();

        public List<ToDoItemDTO> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            var result = itemsCollection.Where(item => { return item.DueDate == dueDate; }).ToList();
            return result;
        }


        public Task UpdateAsync(string s, ToDoItemDTO item)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(ToDoItemDTO newToDoItem)
        {
            throw new NotImplementedException();
        }

        public List<ToDoItemDTO> GetNextFiveDaysItems(DateTimeOffset dueDate)
        {
            throw new NotImplementedException();
        }

    }
}