using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Model
{
    public class ToDoItemMongoDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public bool isDone { get; set; }
        public bool isFavorite { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public List<DateTimeOffset> ModificationDateTimes { get; set; }
    }
}
