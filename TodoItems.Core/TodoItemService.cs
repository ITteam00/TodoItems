using ToDoItem.Api.Models;

namespace TodoItems.Core
{
    public class TodoItemService
    {
        private const int MAX_EDIT_TIMES = 2;
        private const int MAX_DUEDATE = 8;
        private readonly ITodoItemsRepository _todosRepository;

        public TodoItemService(ITodoItemsRepository repository)
        {
            _todosRepository = repository;
        }

        public async Task<ToDoItemObj> CreateToDoItem(ToDoItemObj inputToDoItem)
        {

            return await _todosRepository.CreateAsync(inputToDoItem);
        }

        public async Task<ToDoItemObj> EditToDoItem(ToDoItemObj inputToDoItem)
        {
            String id = inputToDoItem.Id;
            var existingItem = await _todosRepository.FindById(id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException("Todo item not found.");
            }

            existingItem.Description = inputToDoItem.Description;
            existingItem.Favorite = inputToDoItem.Favorite;
            existingItem.DueDate = inputToDoItem.DueDate;

            return await _todosRepository.EditItem(existingItem);
        }

        public async Task<ToDoItemObj> GetToDoItemById(string id)
        {
            return await _todosRepository.FindById(id);
        }

        public async Task<List<ToDoItemObj>> GetAllToDoItemsInOneDay(DateTime date)
        {
            return await _todosRepository.FindAllTodoItemsInOneDay(date);
        }


    }
}
