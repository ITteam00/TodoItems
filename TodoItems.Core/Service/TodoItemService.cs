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
        if (item.DueTime == DateTime.MinValue)
        {
            var dueDateGenerateStrategy = new DueDateGenerator().getStrategy(type);
            List<TodoItem> nextFiveDaysItem = await _todoRepository.getNextFiveDaysItem(item.CreateTime.Date);
            Dictionary<DateTime, List<TodoItem>> dueDateDic = countTodoItemByDueDate(nextFiveDaysItem);
            DateTime dueDate = dueDateGenerateStrategy.generateDueDate(dueDateDic);
            item.DueTime = dueDate;
            await _todoRepository.SaveAsync(item);
        }
        else
        {
            List<TodoItem> todoItems = await _todoRepository.getAllItemsCountInToday(item.DueTime.Date);
            if (todoItems.Count > 8)
            {
                throw new TooManyTodoItemInDueDateException("A maximum of eight todoitems can be completed per day");
            }
            else
            if (item.DueTime.Date < DateTime.Now.Date)
            {
                throw new DueDateEarlierThanCreateDateException("Due time should later than create time");
            }
            await _todoRepository.SaveAsync(item);
        }
        return item;
    }

    private Dictionary<DateTime, List<TodoItem>> countTodoItemByDueDate(List<TodoItem> items)
    {
        Dictionary<DateTime, List<TodoItem>> todoItemByDueDate = new Dictionary<DateTime, List<TodoItem>>();
        foreach (TodoItem item in items)
        {
            if (todoItemByDueDate.ContainsKey(item.DueTime.Date))
            {
                todoItemByDueDate[item.DueTime.Date].Add(item);
            }
            else
            {
                todoItemByDueDate.Add(item.DueTime.Date, [item]);
            }
        }
        return todoItemByDueDate;
    }


    public async Task<TodoItem> ModifyTodoItem(string id, TodoItem newTodoItem)
    {
        TodoItem item = await _todoRepository.FindById(id);
        item.Modify(newTodoItem);
        await _todoRepository.SaveAsync(item);
        return item;
    }
}
