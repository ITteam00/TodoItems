namespace TodoItems.Core
{
    public class TodoItem
    {
        public DateTime? CreateTime { get; init; } = DateTime.Now;
        public string Description { get; set; }
        public DateTime DueTime { get; set; }
        public string? Id { get; init; }
        public Boolean IsComplete { get; set; }
        public Boolean IsFavorite { get; set; }
        public DateTime[]? ModifyTime { get; set; }
    }
}