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
        var TimeStamps = new List<DateTime>();
        Assert.Equal(true, todoItem.ModifyItem(TimeStamps, todoItem.Id));
    }
    [Fact]
    public void Should_return_false_when_modify_item_Forth_time()
    {
        var todoItem = new TodoItem();
        todoItem.Id = "1";

        var TimeStamps = new List<DateTime>();

        Assert.Equal(true, todoItem.ModifyItem(TimeStamps, todoItem.Id));
    }
}