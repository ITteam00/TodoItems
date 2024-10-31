using ToDoItem.Api.Models;

namespace TodoItems.Core;

public class TodoItemService
{
    private readonly ITodosRepository todosRepository;


    public TodoItemService(ITodosRepository repository)
    {
        todosRepository = repository;
    }


    public async Task<ToDoItemDto> OnDetectEdit(ToDoItemDto item)
    {
        DateTime lastModifiedDate = item.LastModifiedTimeDate;
        DateTime currentDate = DateTimeOffset.Now.Date;
        TimeSpan difference = currentDate - lastModifiedDate;
        if (difference.Days >= 1)
        {
            ToDoItemDto newItem =  await Task.FromResult(AddEditTimes(item));
            return newItem;
        }
        if (item.EditTimes <= 2)
        {
            ToDoItemDto newItem = await Task.FromResult(AddEditTimes(item));
            return newItem;
        }
        else {
            throw new InvalidOperationException("Too many edits");

        }


    }

    public async Task<ToDoItemDto> CreateAsync(ToDoItemDto inputToDoItemModel)
    {
        if (inputToDoItemModel.DueDate < inputToDoItemModel.CreatedTimeDate)
        {
            throw new InvalidOperationException("due date cannot be before creation date");
        }

        var todayItems = todosRepository.findAllTodoItemsInToday();
        if (todayItems.Count >= 8)
        {
            throw new InvalidOperationException("Cannot add more than 8 ToDo items for today.");
        }

        return inputToDoItemModel;
    }


    public ToDoItemDto AddEditTimes( ToDoItemDto item)
    {
        var modifiedToDoItemModel = new ToDoItemDto
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



}
