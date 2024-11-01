namespace ToDoItem.Api.Models
{
    public record ToDoItemDto
    {
        public required string Id { get; init; }
        public required string Description { get; set; }
        public required bool Done { get; set; }
        public required bool Favorite { get; set; }
        public required DateTime CreatedTimeDate { get; init; }
        public required DateTime LastModifiedTimeDate { get; set; }
        public required int EditTimes { get; set; }
        public DateTime? DueDate { get; set; } 




        public static implicit operator Task<object>(ToDoItemDto v)
        {
            throw new NotImplementedException();
        }

        //public async Task<ToDoItemDto> ModifyItem(ToDoItemDto item)
        //{

        //}

        //public async Task<ToDoItemDto> CreateAsync(ToDoItemDto inputToDoItemModel)
        //{

        //}

        //public ToDoItemDto AddEditTimes(ToDoItemDto item)
        //{

        //}



    }
}
