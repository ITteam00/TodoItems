using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.Validator;

namespace TodoItems.Test
{

    public class TodoItemValidatorTest
    {
        private readonly TodoItemValidator _todoItemValidator;
        public TodoItemValidatorTest()
        {
            _todoItemValidator = new TodoItemValidator();
        }

        [Fact]
        public void Should_return_modificationCount_when_call_ModificationCount()
        {
            List<DateTimeOffset> dateTimes = new List<DateTimeOffset>
            {
                new DateTimeOffset(2024, 10, 30, 14, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 9, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 10, 29, 18, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 17, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 16, 2, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 11, 1, 16, 2, 0, TimeSpan.Zero)
            };

            Assert.Equal(4, _todoItemValidator.ModificationCount(dateTimes));
        }


    }
}
