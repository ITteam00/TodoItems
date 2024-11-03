using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    internal interface IDueDateSelectStrategy
    {
        DateTime generateDueDate(Dictionary<DateTime,List<TodoItem>> dueDateDic);
    }
}
