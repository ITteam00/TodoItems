using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Service;

namespace TodoItems.Core
{
    public enum DueDateStrategyType
    {
        Earliest,
        FewestCompleted
    }
    public class TodoItemService
    {
        public ITodosRepository Repo;
        public int CompletedLimit { get; set; } = 8;
        public TodoItemService(ITodosRepository repo) {
            Repo = repo;
        }
        public TodoItem Create(string id, string description, DateTimeOffset? dueDate = null, DueDateStrategyType? dueStrategyType=null)
        {
            DueDateStrategy dueDateStrategy = new DueDateStrategy();  // todo use stragy pattern refractor
            var item = new TodoItem
            {
                Id = id,
                Description = description,
                DueDate = dueDate,
            };
            if (item.DueDate != null)
            {
                ValidateDueDate(item);
            } else
            {
                var fiveDayItems = Repo.GetFiveDayItems();
                if(dueStrategyType != DueDateStrategyType.Earliest)
                {
                    var dueDateResult = dueDateStrategy.GetEarliestDate(fiveDayItems, CompletedLimit);
                    item.DueDate = dueDateResult;
                } else
                {
                    var dueDateResult = dueDateStrategy.GetFewestCompleted(fiveDayItems, CompletedLimit);
                    item.DueDate = dueDateResult;
                }
            }
            return Repo.AddItem(item);
        }

        private void ValidateDueDate(TodoItem item)
        {
            var today = DateTimeOffset.Now.Date;
            if (item.DueDate.Value.Date < today)
            {
                throw new InvalidOperationException("Item due date is earlier than today.");
            }
            var findResult = Repo.GetItemsByDueDate(item.DueDate.Value);
            if (findResult.Count > CompletedLimit)
            {
                throw new InvalidOperationException($"Item due date limit reached for today.");
            }
        }
    }
}
