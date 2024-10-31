using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public interface ITodosRepository
    {
        List<ToDoItemModel> findAllTodoItemsInToday();
    }
}
