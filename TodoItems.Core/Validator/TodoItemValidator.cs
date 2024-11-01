namespace TodoItems.Core.Validator
{
    public class TodoItemValidator
    {
        public int ModificationCount(List<DateTimeOffset> modificationDateTimes)
        {
            var count = modificationDateTimes.Count(d => d.Date == DateTimeOffset.UtcNow.Date);
            return count;
        }
    }
}
