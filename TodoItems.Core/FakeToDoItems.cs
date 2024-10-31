using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoItem.Api.Models;

var fakeToDoItems = new List<ToDoItemModel>
        {
            new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Item 1",
                Done = false,
                Favorite = false,
                CreatedTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EditTimes = 0
            },
            new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Item 2",
                Done = true,
                Favorite = true,
                CreatedTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EditTimes = 1
            },
            new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Item 3",
                Done = false,
                Favorite = true,
                CreatedTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EditTimes = 2
            },
            new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Item 4",
                Done = true,
                Favorite = false,
                CreatedTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EditTimes = 3
            },
            new ToDoItemModel
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Item 5",
                Done = false,
                Favorite = false,
                CreatedTime = DateTimeOffset.Now,
                LastModifiedTime = DateTimeOffset.Now,
                EditTimes = 4
            }
        };

