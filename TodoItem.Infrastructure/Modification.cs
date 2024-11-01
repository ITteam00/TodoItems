namespace TodoItem.Infrastructure
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