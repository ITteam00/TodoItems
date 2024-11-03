using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Model;

namespace TodoItems.Core.Strategy
{
    public class ClosestDueDateStrategy : IDueDateSelectStrategy
    {
        public DateTime generateDueDate(Dictionary<DateTime, List<TodoItem>> dueDateDic)
        {
            var result = dueDateDic.Where(dueDateItemsPairs => dueDateItemsPairs.Value.Count < 8).
                OrderBy(dueDateItemsPairs => Math.Abs((dueDateItemsPairs.Key - DateTime.Now).TotalDays))
                .FirstOrDefault();
            if (result.Equals(default(KeyValuePair<DateTime, List<TodoItem>>)))
            {
                throw new NoSuitableDueDateException("No suitable due date found.");

            }
            return result.Key;
        }
    }
}

