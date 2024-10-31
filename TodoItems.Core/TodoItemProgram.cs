using ToDoItem.Api.Models;

namespace TodoItems.Core;

public class TodoItemProgram
{


    public void OnDetectEdit(ToDoItemModel item)
    {
        //AddEditTimes(item);
        return;
    }


    public ToDoItemModel AddEditTimes( ToDoItemModel item)
    {
        var modifiedToDoItemModel = new ToDoItemModel
        {
            Description = item.Description,
            Id = Guid.NewGuid().ToString(),
            Favorite = item.Favorite,
            Done = item.Done,
            //CreatedTime = DateTimeOffset.Now,
            CreatedTime = item.LastModifiedTime,
            LastModifiedTime = item.LastModifiedTime,
            EditTimes = 0,

        };

        return modifiedToDoItemModel;

    }

    public string AlertMessage(ToDoItemModel item)
    {
        return "Your edit time run out";
    }

}
