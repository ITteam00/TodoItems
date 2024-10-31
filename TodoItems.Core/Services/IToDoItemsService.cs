using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public interface IToDoItemsService
    {
        Task UpdateAsync(string id, ToDoItemDto updatedToDoItem);
    }
}
