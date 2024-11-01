using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TodoItems.Core.Model;
using DnsClient.Internal;

namespace TodoItems.Core.Services
{
    public class ToDoItemsService : IToDoItemsService
    {
        private ITodosRepository _todosRepository;
        private int MaximumModificationNumber = 8;

        public ToDoItemsService(ITodosRepository todosRepository)
        {
            _todosRepository = todosRepository;
        }

        public int ModificationCount(List<DateTimeOffset> modificationDateTimes)
        {
            var count = modificationDateTimes.Count(d => d.Date == DateTimeOffset.UtcNow.Date);
            return count;
        }

        public async Task CheckCountUpdateAsync(string id, ToDoItemDTO updatedToDoItem)
        {
            var modificationCount = ModificationCount(updatedToDoItem.ModificationDateTimes);
            if (modificationCount >= 3)
            {
                throw new TooManyEntriesException("to many");
            }

            await _todosRepository.UpdateAsync(id, updatedToDoItem);
        }

        public bool CanModify(ToDoItemDTO updatedToDoItem)
        {
            var ModifiedTimes = updatedToDoItem.ModificationDateTimes;
            ModifiedTimes = ModifiedTimes.Where(t => IsTodady(t)).ToList();
            if (ModifiedTimes.Count >= MaximumModificationNumber)
            {
                return false;
            }

            return true;
        }

        public bool IsTodady(DateTimeOffset dateTime)
        {
            var toady = DateTimeOffset.Now.Date;
            return dateTime.Date == toady;
        }

        public async Task UpdateAsync(string id, ToDoItemDTO updatedToDoItem)
        {
            var item = new ToDoItemMongoDTO
            {
                Id = id,
                Description = updatedToDoItem.Description,
                isDone = updatedToDoItem.IsDone,
                isFavorite = updatedToDoItem.IsFavorite,
                CreatedTime = updatedToDoItem.CreatedTime,
            };
            await _todosRepository.UpdateAsync(id, item);
        }


        public async Task CreateAsync(ToDoItemDTO newToDoItem)
        {
            var item = new ToDoItemMongoDTO
            {
                Id = newToDoItem.Id,
                Description = newToDoItem.Description,
                isDone = newToDoItem.IsDone,
                isFavorite = newToDoItem.IsFavorite,
                CreatedTime = newToDoItem.CreatedTime,
            };

            await _todosRepository.CreateAsync(item);
            ;
        }
    }

    public class TooManyEntriesException : Exception
    {
        public TooManyEntriesException(string message) : base(message)
        {
        }
    }
}