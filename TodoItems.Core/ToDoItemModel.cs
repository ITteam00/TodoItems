namespace ToDoItem.Api.Models
{
    public record ToDoItemModel
    {
        public required string Id { get; init; }
        public required string Description { get; set; }
        public required bool Done { get; set; }
        public required bool Favorite { get; set; }
        public required DateTime CreatedTimeDate { get; init; }
        public required DateTime LastModifiedTimeDate { get; init; }
        public required int EditTimes { get; init; }



        public static implicit operator Task<object>(ToDoItemModel v)
        {
            throw new NotImplementedException();
        }
    }
}
