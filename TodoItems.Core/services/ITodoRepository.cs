using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.services
{
    public interface ITodoRepository
    {
        public List<TodoItem> getAllItemsCountInToday(DateTime today);
    }
}
