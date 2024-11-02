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
        Task<List<TodoItemDTO>> GetItemsByDueDate(DateTimeOffset? dueDate);
        Task UpdateAsync(string s, TodoItemDTO item);
        Task CreateAsync(TodoItemDTO newTodoItem);
        List<TodoItemDTO> GetNextFiveDaysItems(DateTimeOffset dueDate);
    }
}