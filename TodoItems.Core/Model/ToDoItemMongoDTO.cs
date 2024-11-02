using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.Model
{
    public class TodoItemMongoDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public bool IsFavorite { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public List<DateTimeOffset> ModificationDateTimes { get; set; }
        public DateTimeOffset? DueDate { get; set; }
    }
}