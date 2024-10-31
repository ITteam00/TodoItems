namespace TodoItems.Core
{
    public record TodoItemDto
    {

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Id { get; set; }
        public bool IsDone { get; set; }
        public bool IsFavorite { get; set; }
    }
}