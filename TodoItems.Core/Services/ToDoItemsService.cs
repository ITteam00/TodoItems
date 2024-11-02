using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TodoItems.Core.Model;
using DnsClient.Internal;
using TodoItems.Core.Validator;

namespace TodoItems.Core.Services
{
    public class ToDoItemsService
    {
        private readonly ITodosRepository _todosRepository;
        private readonly TodoItemValidator _todoItemValidator;

        public ToDoItemsService(ITodosRepository todosRepository)
        {
            _todosRepository = todosRepository;
            _todoItemValidator = new TodoItemValidator();
        }



        public async Task UpdateAsync(string id, TodoItemDTO updatedTodoItem)
        {
            var modificationCount = _todoItemValidator.ModificationCount(updatedTodoItem.ModificationDateTimes);
            if (modificationCount >= 3)
            {
                throw new TooManyEntriesException("to many");
            }

            await _todosRepository.UpdateAsync(id, updatedTodoItem);
        }


        public async Task CreateAsync(TodoItemDTO createTodoItem,string type)
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