using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core
{
    internal class DueDateGenerator
    {
        public IDueDateSelectStrategy getStrategy(string type)
        {
            return type switch
            {
                "closest" => new ClosestDueDateStrategy(),
                "freest"=>new FreestDueDateStrategy(),
                _=>throw new InvalidDueDateGenerateStrategyException("Invalid daueDate generate strategy")
            };
        }
    }
}
