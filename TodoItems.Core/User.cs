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
                var findResult = Repo.GetItemsByDueDate(item.DueDate.Value);
                if(findResult.Count >8 )
                {
                    throw new InvalidOperationException($"Item due date limit reached for today.");
                }
            }
            return Repo.AddItem(item);
        }
    }
}
