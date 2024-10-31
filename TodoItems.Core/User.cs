using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Service;

namespace TodoItems.Core
{
    public class User
    {
        public int CompletedLimit { get; set; } = 8;
        public ITodosRepository Repo;
        public string Id { get; set; }

        public User(ITodosRepository repo)
        {
            Repo = repo;
        }

        

        public int AddOneItem(TodoItem item)
        {
            if(item.DueDate != null)
            {
                var today = DateTimeOffset.Now.Date;
                if(today<item.DueDate)
                {
                    throw new InvalidOperationException("Item due date is earlier than today.");
                }
                var findResult = Repo.GetItemsByDueDate(item.DueDate.Value);
                if (findResult.Count > CompletedLimit)
                {
                    throw new InvalidOperationException($"Item due date limit reached for today.");
                }
            }
            return Repo.AddItem(item);
        }
    }
}
