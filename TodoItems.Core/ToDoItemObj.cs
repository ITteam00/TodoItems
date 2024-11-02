using TodoItems.Core;

namespace ToDoItem.Api.Models
{
    public enum DueDateRequirementType
    {
        Earliest,
        Fewest
    }

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
        public DueDateRequirementType? DueDateRequirement { get; set; }


        private const int MAX_EDIT_Times = 3;



        public ToDoItemObj(string id, string description, bool done, bool favorite, DateTime createdTimeDate, DateTime lastModifiedTimeDate, int editTimes, DateTime? dueDate, DueDateRequirementType? dueDateRequirement)
        {
            
            Description = description;
            Id = id;
            Done = done;
            Favorite = favorite;
            CreatedTimeDate = createdTimeDate;
            LastModifiedTimeDate = lastModifiedTimeDate;
            EditTimes = editTimes;
            DueDate = dueDate;
            DueDateRequirement = dueDateRequirement;
        }

        public static implicit operator Task<object>(ToDoItemObj v)
        {
            throw new NotImplementedException();
        }

        public void ValidateDueDate()
        {
            if (DueDate < CreatedTimeDate)
            {
                throw new InvalidOperationException("Due date cannot be before creation date");
            }
        }

        public void IncrementEditTimes()
        {
            if (EditTimes >= MAX_EDIT_Times)
            {
                throw new InvalidOperationException("Too many edits");
            }
            EditTimes++;
        }

    }
}
