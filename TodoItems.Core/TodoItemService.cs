using ToDoItem.Api.Models;

namespace TodoItems.Core;

public class TodoItemService
{
    private const int MAX_EDIT_Times = 2;
    private const int MAX_DUEDATE = 8;
    private readonly ITodoItemsRepository todosRepository;


    public TodoItemService(ITodoItemsRepository repository)
    {
        todosRepository = repository;
    }

    //public async void AddToDoItem(ToDoItemDto inputToDoItemModel, ITodosRepository todosRepository)
    //{

    //}




}
