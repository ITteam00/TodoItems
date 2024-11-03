using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Strategy
{
    internal interface IDueDateSelectStrategy
    {
        DateTime generateDueDate(Dictionary<DateTime, List<TodoItem>> dueDateDic);
    }
}
