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
        List<ToDoItemDto> GetItemsByDueDate(DateTimeOffset dueDate);
        int AddItem(ToDoItemDto item);

        Task ReplaceAsync(string s, ToDoItemMongoDTO item);
    }
}
