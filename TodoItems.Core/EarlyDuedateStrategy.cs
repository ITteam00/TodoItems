using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    public class EarlyDuedateStrategy : IDuedateStrategy
    {
        public Task<DateTime?> SetDuedate(TodoItemDto todoItemDto)
        {
            throw new NotImplementedException();
        }
    }
}
