using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace FlubuCore.Context.FluentInterface
{
    public class AddTaskOptions<T>
    {
        public T Param { get; set; }

        public void ForMember(Action<AddTaskOptions<T>> taskMember, string argKey, string help = null)
        {
        }
    }
}
