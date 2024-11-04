using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.DuedateStartegy;

public interface IDuedateStrategy
{
    DateTime SetDuedate(Dictionary<DateTime, int> dueDateCounts);

}

