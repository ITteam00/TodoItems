using ToDoItem.Api.Models;

namespace TodoItems.Core;

public class TodoItemProgram
{


    public async Task<ToDoItemModel> OnDetectEdit(ToDoItemModel item)
    {
        DateTimeOffset lastModifiedDate = item.LastModifiedTime.Date;
        DateTimeOffset currentDate = DateTimeOffset.Now.Date;
        TimeSpan difference = currentDate - lastModifiedDate;
        if (difference.Days >= 1)
        {
            return await Task.FromResult(AddEditTimes(item));
        } 
        else
        {
            throw new Exception("Too many edits");

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
            //CreatedTime = DateTimeOffset.Now,
            CreatedTime = item.LastModifiedTime,
            LastModifiedTime = item.LastModifiedTime,
            EditTimes = 0,

        };

        return modifiedToDoItemModel;

    }



}
