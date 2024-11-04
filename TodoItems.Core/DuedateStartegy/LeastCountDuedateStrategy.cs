using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.DuedateStartegy
{
    public class LeastCountDuedateStrategy : IDuedateStrategy
    {
        public DateTime SetDuedate(Dictionary<DateTime, int> dueDateCounts)
        {
            var minCountDueDate = dueDateCounts
                .Where(kvp => kvp.Value < 8)
                .OrderBy(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .FirstOrDefault();

            if (minCountDueDate.Equals(default(KeyValuePair<DateTime, int>)))
            {
                throw new InvalidOperationException("No valid due date found, all dates have 8 or more items.");
            }

            return minCountDueDate.Key;
        }
    }
}
