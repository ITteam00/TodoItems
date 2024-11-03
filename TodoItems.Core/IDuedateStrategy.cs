using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core;

public interface IDuedateStrategy
{
    DateTime SetDuedate(Dictionary<DateTime, int> dueDateCounts);

}

