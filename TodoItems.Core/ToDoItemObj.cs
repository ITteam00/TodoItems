using TodoItems.Core;

namespace ToDoItem.Api.Models
{
    public class ToDoItemObj
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public bool Favorite { get; set; }
        public DateTime CreatedTimeDate { get; set; }
        public DateTime LastModifiedTimeDate { get; set; }
        public int EditTimes { get; set; }
        public DateTime? DueDate { get; set; }


        private const int MAX_EDIT_Times = 2;
        private const int MAX_DUEDATE = 8;


        public ToDoItemObj(string id, string description, bool done, bool favorite, DateTime createdTimeDate, DateTime lastModifiedTimeDate, int editTimes, DateTime? dueDate)
        {
            
            Description = description;
            Id = id;
            Done = done;
            Favorite = favorite;
            CreatedTimeDate = createdTimeDate;
            LastModifiedTimeDate = lastModifiedTimeDate;
            EditTimes = editTimes;
            DueDate = dueDate;
        }

        public static implicit operator Task<object>(ToDoItemObj v)
        {
            throw new NotImplementedException();
        }

        public async Task<ToDoItemObj> CreateAsync(ToDoItemObj inputToDoItem, ITodosRepository todosRepository)
        {
            if (inputToDoItem.DueDate < inputToDoItem.CreatedTimeDate)
            {
                throw new InvalidOperationException("due date cannot be before creation date");
            }

            var todayItems = todosRepository.findAllTodoItemsInToday();
            if (todayItems.Count >= MAX_DUEDATE)
            {
                throw new InvalidOperationException("Cannot add more than 8 ToDo items for today.");
            }

            return inputToDoItem;
        }

        public async Task<ToDoItemObj> ModifyItem(ToDoItemObj item)
        {
            DateTime lastModifiedDate = item.LastModifiedTimeDate;
            DateTime currentDate = DateTimeOffset.Now.Date;
            TimeSpan difference = currentDate - lastModifiedDate;
            if (difference.Days >= 1)
            {
                item = await Task.FromResult(AddEditTimes(item));
                item.EditTimes = 1;
                return item;
            }
            if (item.EditTimes <= MAX_EDIT_Times)
            {
                item = await Task.FromResult(AddEditTimes(item));
                return item;
            }
            else
            {
                throw new InvalidOperationException("Too many edits");
            }
        }

        public ToDoItemObj AddEditTimes(ToDoItemObj item)
        {
            return new ToDoItemObj(
                item.Id,
                item.Description,
                item.Done,
                item.Favorite,
                item.CreatedTimeDate,
                DateTimeOffset.Now.Date,
                item.EditTimes + 1,
                item.DueDate
            );
        }



    }
}
