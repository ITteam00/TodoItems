namespace TodoItems.Core;

public class TodoItem
{
    public String Id { get; set; }
    public String Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; }
    public bool IsDone { get; set; }
    public bool IsFavorite { get; set; }
}
