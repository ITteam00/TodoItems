using ToDoItem.Api.Models;

namespace TodoItems.Core;

public class TodoItemService
{
    private const int MAX_EDIT_Times = 2;
    private const int MAX_DUEDATE = 8;
    private readonly ITodosRepository todosRepository;


    public TodoItemService(ITodosRepository repository)
    {
        todosRepository = repository;
    }


    public async Task<ToDoItemDto> ModifyItem(ToDoItemDto item)
    {
        DateTime lastModifiedDate = item.LastModifiedTimeDate;
        DateTime currentDate = DateTimeOffset.Now.Date;
        TimeSpan difference = currentDate - lastModifiedDate;
        if (difference.Days >= 1 )
        {
            ToDoItemDto newItem =  await Task.FromResult(AddEditTimes(item));
            newItem.EditTimes = 0;
            return newItem;
        }
        if (item.EditTimes <= MAX_EDIT_Times)
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
        if (todayItems.Count >= MAX_DUEDATE)
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
