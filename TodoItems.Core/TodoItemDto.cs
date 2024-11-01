namespace TodoItems.Core
{
    public class TodoItemDto
    {
        public DateTime? CreateTime { get; set; }
        public string Description { get; set; }
        public DateTime DueTime { get; set; }
        public string? Id { get; set; }
        public Boolean IsComplete { get; set; }
        public Boolean IsFavorite { get; set; }
        public DateTime[]? ModifyTime { get; set; }
    }
}