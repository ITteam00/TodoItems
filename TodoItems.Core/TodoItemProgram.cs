using ToDoItem.Api.Models;

namespace TodoItems.Core;

public class TodoItemProgram
{
    private readonly ITodosRepository todosRepository;

    public TodoItemProgram(ITodosRepository repository)
    {
        todosRepository = repository;
    }

    public async Task<ToDoItemModel> OnDetectEdit(ToDoItemModel item)
    {
        DateTime lastModifiedDate = item.LastModifiedTimeDate;
        DateTime currentDate = DateTimeOffset.Now.Date;
        TimeSpan difference = currentDate - lastModifiedDate;
        if (difference.Days >= 1)
        {
            ToDoItemModel newItem =  await Task.FromResult(AddEditTimes(item));
            return newItem;
        }
        if (item.EditTimes <= 2)
        {
            ToDoItemModel newItem = await Task.FromResult(AddEditTimes(item));
            return newItem;
        }
        else {
            throw new InvalidOperationException("Too many edits");

        }


    }


    public ToDoItemModel AddEditTimes( ToDoItemModel item)
    {
        var modifiedToDoItemModel = new ToDoItemModel
        {
            Description = item.Description,
            Id = item.Id,
            Favorite = item.Favorite,
            Done = item.Done,
            CreatedTimeDate = item.CreatedTimeDate,
            LastModifiedTimeDate = DateTimeOffset.Now.Date,
            EditTimes = item.EditTimes + 1,
            DueDate = item.DueDate

};

        return modifiedToDoItemModel;

    }

    public bool AddDueDate(ToDoItemModel item)
    {
        var todayItems = todosRepository.findAllTodoItemsInToday();
        if (todayItems.Count >= 8)
        {
            throw new InvalidOperationException("Cannot add more than 8 ToDo items for today.");
        }

        return true;
    }



}
