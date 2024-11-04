using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Strategy;
using TodoItems.Core.Model;

namespace TodoItems.Test
{
    public class DueDateStrategyTest
    {
        [Fact]
        public void should_return_freest_date_when_multiple_date_valid()
        {
            var dueDateGenerateStrategy=new FreestDueDateStrategy();
            DateTime today= DateTime.Now.Date;
            var dueDateDic = new Dictionary<DateTime, List<TodoItem>>
            {
                {today.AddDays(1),[new TodoItem(), new TodoItem(), new TodoItem()] },
                {today.AddDays(2),[new TodoItem()] },
                {today.AddDays(3),[new TodoItem(), new TodoItem(), new TodoItem()
                ,new  TodoItem(),new TodoItem(),new  TodoItem()
                ,new  TodoItem(),new TodoItem(),new TodoItem()] },
                {today.AddDays(4),[new TodoItem(), new TodoItem()] },
            };
            var result = dueDateGenerateStrategy.generateDueDate(dueDateDic);
            Assert.Equal(today.AddDays(2),result);
        }

        [Fact]
        public void should_return_closest_date_when_multiple_date_valid()
        {
            var dueDateGenerateStrategy = new ClosestDueDateStrategy();
            DateTime today = DateTime.Now.Date;
            var dueDateDic = new Dictionary<DateTime, List< TodoItem>>
            {
                {today.AddDays(1),[new TodoItem(), new TodoItem(), new TodoItem()] },
                {today.AddDays(2),[new TodoItem()] },
                {today.AddDays(3),[new TodoItem(), new TodoItem(), new TodoItem()
                ,new  TodoItem(),new TodoItem(),new TodoItem()
                ,new  TodoItem(),new TodoItem(),new  TodoItem()] },
                {today.AddDays(4),[new TodoItem(), new TodoItem()] },
            };
            var result = dueDateGenerateStrategy.generateDueDate(dueDateDic);
            Assert.Equal(today.AddDays(1), result);
        }
    }
}
