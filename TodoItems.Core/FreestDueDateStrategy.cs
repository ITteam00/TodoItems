using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    internal class FreestDueDateStrategy:IDueDateSelectStrategy
    {
        public DateTime generateDueDate(Dictionary<DateTime, List<TodoItem>> dueDateDic)
        {
            return dueDateDic.Where(dueDateItemsPairs=>dueDateItemsPairs.Value.Count<8)
                .OrderBy(dueDateItemsPairs=>dueDateItemsPairs.Value.Count)
                .ThenBy(dueDateItemsPairs=>dueDateItemsPairs.Key)
                .FirstOrDefault().Key;
        }
    }
}
