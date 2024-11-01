
namespace TodoItems.Core
{
    interface IDueDateStrategy
    {
        public DateTimeOffset GetEarliestDate(List<TodoItem> fiveDayItems, int completedLimit);
        public DateTimeOffset GetFewestCompleted(List<TodoItem> fiveDayItems, int completedLimit);


    }

    public class DueDateStrategy : IDueDateStrategy
    {
        public DateTimeOffset GetFewestCompleted(List<TodoItem> fiveDayItems, int completedLimit)
        {
            if(fiveDayItems == null || fiveDayItems.Count == 0)
            {
                return DateTimeOffset.Now;
            }
            var result = fiveDayItems
                .Where(item => item.DueDate.HasValue)
                .GroupBy(item => item.DueDate.Value.Date)
                .Where(dueDateGroup => dueDateGroup.Count() <= completedLimit)
                .OrderBy(dueDateGroup => dueDateGroup.Count())
                .ThenBy(group => group.Key)
                .FirstOrDefault();
            if(result == null)
            {
                throw new InvalidOperationException("No fewest completed due date found with item count less than or equal to the completed limit.");
            }
            return result.Key;
        }

        public DateTimeOffset GetEarliestDate(List<TodoItem> fiveDayItems, int completedLimit)
        {
            if (fiveDayItems == null || fiveDayItems.Count == 0)
            {
                return DateTimeOffset.Now;
            }
            var result = fiveDayItems
                .Where(item => item.DueDate.HasValue)
                .GroupBy(item => item.DueDate.Value.Date)
                .Where(dueDateGroup => dueDateGroup.Count() <= completedLimit)
                .OrderBy(dueDateGroup => dueDateGroup.Key)
                .FirstOrDefault();
            if(result == null)
            {
                throw new InvalidOperationException("No earliest due date found with item count less than or equal to the completed limit.");
            }
            return result.Key;
            
        }
    }
}