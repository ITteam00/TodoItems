using System.Runtime.CompilerServices;
using TodoItems.Core.Model;
using TodoItems.Core.Strategy;


namespace TodoItems.Core.service;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoItemsRepository _todoRepository;

    public TodoItemService(ITodoItemsRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<TodoItem> CreateTodoItem(TodoItem item, string? type = "")
    {
        if (item.DueDate == DateTime.MinValue)
        {
            var dueDateGenerateStrategy = new DueDateGenerator().getStrategy(type);
            List<TodoItem> nextFiveDaysItem = await _todoRepository.GetNextFiveDaysItem(item.CreateTime.Date);
            Dictionary<DateTime, List<TodoItem>> dueDateDic = countTodoItemByDueDate(nextFiveDaysItem);
            DateTime dueDate = dueDateGenerateStrategy.generateDueDate(dueDateDic);
            item.DueDate = dueDate;
            await _todoRepository.CreateTodoItemAsync(item);
        }
        else
        {
            List<TodoItem> todoItems = await _todoRepository.GetAllItemsInDueDate(item.DueDate.Date);
            if (todoItems.Count > 8)
            {
                throw new TooManyTodoItemInDueDateException("A maximum of eight todoitems can be completed per day");
            }
            else
            if (item.DueDate.Date < DateTime.Now.Date)
            {
                throw new DueDateEarlierThanCreateDateException("Due time should later than create time");
            }
            await _todoRepository.CreateTodoItemAsync(item);
        }
        return item;
    }

    private Dictionary<DateTime, List<TodoItem>> countTodoItemByDueDate(List<TodoItem> items)
    {
        Dictionary<DateTime, List<TodoItem>> todoItemByDueDate = new Dictionary<DateTime, List<TodoItem>>();
        foreach (TodoItem item in items)
        {
            if (todoItemByDueDate.ContainsKey(item.DueDate.Date))
            {
                todoItemByDueDate[item.DueDate.Date].Add(item);
            }
            else
            {
                todoItemByDueDate.Add(item.DueDate.Date, [item]);
            }
        }
        return todoItemByDueDate;
    }


    public async Task<TodoItem> ModifyTodoItem(string id, TodoItem newTodoItem)
    {
        TodoItem item = await _todoRepository.FindById(id);
        item.Modify(newTodoItem);
        await _todoRepository.UpdateAsync(id,item);
        return item;
    }
}
