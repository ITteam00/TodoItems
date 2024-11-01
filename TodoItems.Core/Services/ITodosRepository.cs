using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public interface ITodosRepository
    {
        List<ToDoItemDTO> GetItemsByDueDate(DateTimeOffset dueDate);
        Task UpdateAsync(string s, ToDoItemDTO item);
        Task CreateAsync(ToDoItemDTO newToDoItem);
        List<ToDoItemDTO> GetNextFiveDaysItems(DateTimeOffset dueDate);
    }
}