using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    public class TooManyTodoItemInDueDateException : Exception
    {
        public TooManyTodoItemInDueDateException(string message):base(message= "A maximum of eight todoitems can be completed per day")
        {}
    }

    public class DueDateEarlierThanCreateDateException : Exception
    {
        public DueDateEarlierThanCreateDateException(string message) : base(message="Due time should later than create time")
        {}
    }

    public class InvalidDueDateGenerateStrategyException : Exception
    {
        public InvalidDueDateGenerateStrategyException(string message) : base(message = "Invalid daueDate generate strategy")
        { }
    }

    public class NoSuitableDueDateException : Exception
    {
        public NoSuitableDueDateException(string message) : base(message = "No suitable due date found")
        { }
    }
}
