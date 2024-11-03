namespace TodoItems.Core
{
    public class TodoItem
    {
        public DateTime CreateTime { get; init; } = DateTime.Now;
        public string Description { get; set; }
        public DateTime DueTime { get; set; }
        public string? Id { get; init; }=Guid.NewGuid().ToString();
        public Boolean IsComplete { get; set; }
        public Boolean IsFavorite { get; set; }
        public DateTime[]? ModifyTime { get; set; } = [DateTime.Now];

        public void Modify(TodoItem newTodoItem)
        {
            if (this.ModifyTime[this.ModifyTime.Length - 1].Date != DateTime.Now.Date)
            {
                this.ModifyTime = [DateTime.Now];
                this.Description = newTodoItem.Description;
                this.IsComplete = newTodoItem.IsComplete;
                this.IsFavorite = newTodoItem.IsFavorite;
            }
            else
            {
                if (this.ModifyTime.Length >= 3)
                {
                    throw new NoModifyTimeException("No modify time");
                }
                else
                {
                    this.Description = newTodoItem.Description;
                    this.IsComplete = newTodoItem.IsComplete;
                    this.IsFavorite = newTodoItem.IsFavorite;
                    this.ModifyTime.Append(DateTime.Now);
                }
            }
        }
    }


}