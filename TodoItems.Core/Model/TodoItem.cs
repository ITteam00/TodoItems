namespace TodoItems.Core.Model
{
    public class TodoItem
    {
        private const int ModifyTimeLimited = 3;

        public DateTime CreateTime { get; init; } = DateTime.Now;
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Id { get; init; } = Guid.NewGuid().ToString();
        public bool IsComplete { get; set; }
        public bool IsFavorite { get; set; }
        public List<DateTime>? ModifyTime { get; set; }

        public void Modify(TodoItem newTodoItem)
        {
            if (ModifyTime[ModifyTime.Count() - 1].Date != DateTime.Now.Date)
            {
                ModifyTime = [DateTime.Now];
                Description = newTodoItem.Description;
                IsComplete = newTodoItem.IsComplete;
                IsFavorite = newTodoItem.IsFavorite;
            }
            else
            {
                if (ModifyTime.Count() >= ModifyTimeLimited)
                {
                    throw new NoModifyTimeException("No modify time");
                }
                else
                {
                    Description = newTodoItem.Description;
                    IsComplete = newTodoItem.IsComplete;
                    IsFavorite = newTodoItem.IsFavorite;
                    ModifyTime.Add(DateTime.Now);
                    //ModifyTime.Append(DateTime.Now);
                }
            }
        }
    }


}