using Microsoft.VisualStudio.TestPlatform.TestHost;
using TodoItems.Core;

namespace TodoItems.Test;

public class TodoItemTest
{
    [Fact]
    public void should_return_2_when_add_1_1()
    {
        var todoItem = new TodoItem();
        Assert.Equal("1", todoItem.GetId());
    }
    [Fact]
    public void Should_return_True_when_modify_item_third_time()
    {
        var todoItem = new TodoItem();
        todoItem.Id = "1";
        todoItem.CreatedDate = DateTime.Now;

        var TimeStamps = new List<DateTime>();
        Assert.Equal(true, todoItem.ModifyItem(TimeStamps, todoItem.CreatedDate, todoItem.Id));
    }
    [Fact]
    public void Should_return_false_when_modify_item_Forth_time()
    {
        var todoItem = new TodoItem();
        todoItem.Id = "1";
        todoItem.CreatedDate = DateTime.Now;

        var TimeStamps = new List<DateTime>();

        Assert.Equal(true, todoItem.ModifyItem(TimeStamps, todoItem.CreatedDate, todoItem.Id));
    }

    [Fact]
    public void Should_timestamp_length_equal_1_when_update_item()
    {
        var todoItem = new TodoItem();
        todoItem.Id = "1";
        todoItem.CreatedDate = DateTime.Now;
        todoItem.TimeStamps = new List<DateTime>();
        todoItem.UpdateItem(todoItem.Id, todoItem.CreatedDate);
        Assert.Equal(1, todoItem.TimeStamps.Count);
    }

    [Fact]
    public void Should_return_true_when_dayoffset_bigger_than_1()
    {
        var todoItem = new TodoItem();
        DateTime date1 = new DateTime(2024, 10, 30);
        DateTime date2 = new DateTime(2024, 10, 31);
        Assert.Equal(true, todoItem.AreDatesOneDayApart(date1, date2));
    }
}