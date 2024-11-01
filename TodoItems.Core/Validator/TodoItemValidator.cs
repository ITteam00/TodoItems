using TodoItems.Core.Model;

namespace TodoItems.Core.Validator
{
    public class TodoItemValidator
    {
        public int ModificationCount(List<DateTimeOffset> modificationDateTimes)
        {
            var count = modificationDateTimes.Count(d => d.Date == DateTimeOffset.UtcNow.Date);
            return count;
        }

        public Dictionary<DateTime, int> CountDueDates(List<TodoItemDTO> todoItems)
        {
            if (todoItems == null)
            {
                throw new ArgumentNullException(nameof(todoItems));
            }

            var dueDateCounts = todoItems
                .Where(item => item.DueDate.HasValue)
                .GroupBy(item => item.DueDate.Value.Date)
                .ToDictionary(group => group.Key, group => group.Count());

            return dueDateCounts;
        }
    }
}
