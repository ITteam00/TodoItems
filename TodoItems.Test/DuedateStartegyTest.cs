using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core;
using TodoItems.Core.DuedateStartegy;

namespace TodoItems.Test;

public class DuedateStartegyTest
{
    [Fact]
    public async void Should_return_early_Duedate_when_create_item_auto_set_in_five_days_query_none()
    {
        var earlyDuedateStrategy = new EarlyDuedateStrategy();
        var dueDateCounts = new Dictionary<DateTime, int>();
        for (var i = 0; i < 5; i++)
        {
            dueDateCounts.Add(new DateTime(2024, 10, 10+i), 0);
        }
        DateTime RealDueDate = earlyDuedateStrategy.SetDuedate(dueDateCounts);
        DateTime ExpectedDuedate = new DateTime(2024, 10, 10);
        Assert.Equal(ExpectedDuedate.Date, RealDueDate.Date);
    }

    [Fact]
    public async void Should_return_second_early_Duedate_when_create_item_auto_set_in_five_days()
    {
        var earlyDuedateStrategy = new EarlyDuedateStrategy();
        var dueDateCounts = new Dictionary<DateTime, int>();
        dueDateCounts.Add(new DateTime(2024, 10, 10), 8);
        for (var i = 0; i < 4; i++)
        {
            dueDateCounts.Add(new DateTime(2024, 10, 11 + i), 0);
        }
        DateTime RealDueDate = earlyDuedateStrategy.SetDuedate(dueDateCounts);
        DateTime ExpectedDuedate = new DateTime(2024, 10, 11);
        Assert.Equal(ExpectedDuedate.Date, RealDueDate.Date);
    }


    [Fact]
    public async void Should_return_least_count_Duedate_when_SetDuedate_query_none()
    {
        var leastCountDuedateStrategy = new LeastCountDuedateStrategy();
        var dueDateCounts = new Dictionary<DateTime, int>();
        for (var i = 0; i < 5; i++)
        {
            dueDateCounts.Add(new DateTime(2024, 10, 10 + i), 0);
        }
        DateTime RealDueDate = leastCountDuedateStrategy.SetDuedate(dueDateCounts);
        DateTime ExpectedDuedate = new DateTime(2024, 10, 10);
        Assert.Equal(ExpectedDuedate.Date, RealDueDate.Date);
    }

    [Fact]
    public async void Should_return_least_count_Duedate_when_SetDuedate()
    {
        var leastCountDuedateStrategy = new LeastCountDuedateStrategy();
        var dueDateCounts = new Dictionary<DateTime, int>();
        dueDateCounts.Add(new DateTime(2024, 10, 10), 6);
        dueDateCounts.Add(new DateTime(2024, 10, 11), 4);
        dueDateCounts.Add(new DateTime(2024, 10, 12), 6);
        dueDateCounts.Add(new DateTime(2024, 10, 13), 7);
        dueDateCounts.Add(new DateTime(2024, 10, 14), 5);

        DateTime RealDueDate = leastCountDuedateStrategy.SetDuedate(dueDateCounts);
        DateTime ExpectedDuedate = new DateTime(2024, 10, 11);
        Assert.Equal(ExpectedDuedate.Date, RealDueDate.Date);
    }
}

