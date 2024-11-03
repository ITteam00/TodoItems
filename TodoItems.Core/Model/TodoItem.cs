﻿namespace TodoItems.Core.Model
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
        public DateTime[]? ModifyTime { get; set; } = [DateTime.Now];

        public void Modify(TodoItem newTodoItem)
        {
            if (ModifyTime[ModifyTime.Length - 1].Date != DateTime.Now.Date)
            {
                ModifyTime = [DateTime.Now];
                Description = newTodoItem.Description;
                IsComplete = newTodoItem.IsComplete;
                IsFavorite = newTodoItem.IsFavorite;
            }
            else
            {
                if (ModifyTime.Length >= ModifyTimeLimited)
                {
                    throw new NoModifyTimeException("No modify time");
                }
                else
                {
                    Description = newTodoItem.Description;
                    IsComplete = newTodoItem.IsComplete;
                    IsFavorite = newTodoItem.IsFavorite;
                    ModifyTime.Append(DateTime.Now);
                }
            }
        }
    }


}