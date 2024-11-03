using ToDoItem.Api.Models;
using TodoItems.Core;

namespace TodoItem.Infrastructure
{
    public class DueDateStrategy
    {
        private const int MAX_DUEDATE = 8;
        private readonly ITodoItemsRepository _repository;

        public DueDateStrategy(ITodoItemsRepository repository)
        {
            _repository = repository;
        }

        public DateTime DetermineDueDate(ToDoItemObj inputToDoItem)
        {
            if (inputToDoItem.DueDate != null)
            {
                inputToDoItem.ValidateDueDateIsAfterCreatedTime();
                var itemsDueToday = _repository.findAllTodoItemsInOneday((DateTime)inputToDoItem.DueDate);
                if (itemsDueToday.Count >= MAX_DUEDATE)
                {
                    throw new InvalidOperationException("Cannot add more than 8 ToDo items for today.");
                }
                return (DateTime)inputToDoItem.DueDate;
            }

            int[] itemNumbers = GetItemNumbersForNext5Days();

            if (itemNumbers.All(count => count >= MAX_DUEDATE))
            {
                throw new InvalidOperationException("Next 5 days all have 8 toDoItems");
            }

            DateTime earliestDate = DateTime.UtcNow.Date;
            for (int i = 0; i < itemNumbers.Length; i++)
            {
                if (itemNumbers[i] < MAX_DUEDATE)
                {
                    earliestDate = DateTime.UtcNow.Date.AddDays(i);
                    break;
                }
            }

            int minItems = itemNumbers.Min();
            DateTime fewestDate = DateTime.UtcNow.Date;
            for (int i = 0; i < itemNumbers.Length; i++)
            {
                if (itemNumbers[i] == minItems)
                {
                    fewestDate = DateTime.UtcNow.Date.AddDays(i);
                    break;
                }
            }

            if (inputToDoItem.DueDateRequirement == DueDateRequirementType.Earliest)
            {
                return earliestDate;
            }
            return fewestDate;
        }

        private int[] GetItemNumbersForNext5Days()
        {
            int[] itemNumbers = new int[5];

            for (int i = 0; i < 5; i++)
            {
                DateTime targetDate = DateTime.UtcNow.Date.AddDays(i);
                var itemsOnDate = _repository.findAllTodoItemsInOneday(targetDate);
                itemNumbers[i] = itemsOnDate.Count;
            }

            return itemNumbers;
        }
    }
}
