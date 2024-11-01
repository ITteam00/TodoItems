using TodoItems.Infrastructure;

namespace TodoItems.Infrastructure
{
    public class ModificationDto
    {
        public List<DateTimeOffset> ModifiedTimes { get; set; }
        public ModificationDto()
        {
            ModifiedTimes = new List<DateTimeOffset>();
        }
    }
}