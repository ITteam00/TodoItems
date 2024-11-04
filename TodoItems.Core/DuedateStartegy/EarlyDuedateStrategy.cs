using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.DuedateStartegy
{
    public class EarlyDuedateStrategy : IDuedateStrategy
    {
        public DateTime SetDuedate(Dictionary<DateTime, int> dueDateCounts)
        {
            var earliestDueDate = dueDateCounts
                .Where(kvp => kvp.Value < 8)
                .Select(kvp => kvp.Key)
                .OrderBy(date => date)
                .FirstOrDefault();

            if (earliestDueDate == default)
            {
                throw new InvalidOperationException("No valid due date found, all dates have 8 or more items.");
            }
            return earliestDueDate;
        }
    }
}
