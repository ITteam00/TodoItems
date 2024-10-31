using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TodoItems.Core.Model;

namespace TodoItems.Core.Services
{
    public class ToDoItemsService : IToDoItemsService
    {
        private ITodosRepository _todosRepository;

        public ToDoItemsService(ITodosRepository todosRepository)
        {
            _todosRepository = todosRepository;
        }

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
            var item = new ToDoItemMongoDTO
            {
                Id = id,
                Description = updatedToDoItem.Description,
                isDone = updatedToDoItem.isDone,
                isFavorite = updatedToDoItem.isFavorite,
                CreatedTime = updatedToDoItem.CreatedTime,
            };
            await _todosRepository.ReplaceAsync(id, item);
        }
    }

    public class TooManyEntriesException : Exception
    {
        public TooManyEntriesException(string message) : base(message)
        {
        }
    }
}