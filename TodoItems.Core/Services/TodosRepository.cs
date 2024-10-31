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
        public static List<ToDoItemDto> itemsCollection = new List<ToDoItemDto>();

        public List<ToDoItemDto> GetItemsByDueDate(DateTimeOffset dueDate)
        {
            var result = itemsCollection.Where(item =>
            {
                return item.DueDate == dueDate;
            }).ToList();
            return  result;
        }

        public int AddItem(ToDoItemDto item)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceAsync(string s, ToDoItemMongoDTO item)
        {
            throw new NotImplementedException();
        }
    }
}