namespace TodoItems.Core
{
    public record TodoItemDto
    {

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public String Description { get; set; }
        public DateTime DueDate { get; set; }
        public String Id { get; set; }
        public bool IsDone { get; set; }
        public bool IsFavorite { get; set; }

        public List<DateTime>? TimeStamps { get; set; }
    }
}