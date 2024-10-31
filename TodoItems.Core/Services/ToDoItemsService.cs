using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public class ToDoItemsService : IToDoItemsService
    {
        private readonly IMongoCollection<ToDoItemMongoDTO> _ToDoItemsCollection;

        public int ModificationCount(List<DateTimeOffset> modificationDateTimes)
        {
            var count = modificationDateTimes.Count(d => d.Date == DateTimeOffset.UtcNow.Date);
            return count;
        }

        public async Task CheckCountUpdateAsync(string id, ToDoItemDto updatedToDoItem)
        {
            var modificationCount = ModificationCount(updatedToDoItem.ModificationDateTimes);
            if (modificationCount >= 3)
            {
                throw new TooManyEntriesException("to many");
            }

            await UpdateAsync(id, updatedToDoItem);
        }


        public async Task UpdateAsync(string id, ToDoItemDto updatedToDoItem)
        {
            
        }
    }

    public class TooManyEntriesException : Exception
    {
        public TooManyEntriesException(string message) : base(message)
        {
        }
    }
}