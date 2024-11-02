using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    public class TooManyTodoItemInDueDateException : Exception
    {
        public TooManyTodoItemInDueDateException(string message):base(message)
        {}
    }
}
