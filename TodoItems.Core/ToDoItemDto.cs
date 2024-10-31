namespace ToDoItem.Api.Models
{
    public record ToDoItemDto
    {
        public required string Id { get; init; }
        public required string Description { get; set; }
        public required bool Done { get; set; }
        public required bool Favorite { get; set; }
        public required DateTime CreatedTimeDate { get; init; }
        public required DateTime LastModifiedTimeDate { get; init; }
        public required int EditTimes { get; init; }
        public DateTime? DueDate { get; set; } 




        public static implicit operator Task<object>(ToDoItemDto v)
        {
            throw new NotImplementedException();
        }
    }
}
