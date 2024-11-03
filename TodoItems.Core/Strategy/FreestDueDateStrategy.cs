using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Strategy
{
    internal class FreestDueDateStrategy : IDueDateSelectStrategy
    {
        public DateTime generateDueDate(Dictionary<DateTime, List<TodoItem>> dueDateDic)
        {
            var result = dueDateDic.Where(dueDateItemsPairs => dueDateItemsPairs.Value.Count < 8)
                .OrderBy(dueDateItemsPairs => dueDateItemsPairs.Value.Count)
                .ThenBy(dueDateItemsPairs => dueDateItemsPairs.Key)
                .FirstOrDefault();
            if (result.Equals(default(KeyValuePair<DateTime, List<TodoItem>>)))
            {
                throw new NoSuitableDueDateException("No suitable due date found.");
            }
            return result.Key;
        }
    }
}
