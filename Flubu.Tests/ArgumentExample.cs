using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Flubu.Tests
{
    public class Tests
    {
        [Fact]
        public void ValueFromArgumentTest()
        {
            SomeTask task = new SomeTask();
            task.AddArgMapping(x => x.AddPath("default path"), "-p", "help bla bla");
            Assert.Equal("Path from arg", task.Path);
        }

        [Fact]
        public void DefaultValuetTest()
        {
            SomeTask task = new SomeTask();
            task.AddArgMapping(x => x.AddPath("default path"), "-nonexist");
            Assert.Equal("default path", task.Path);
        }
    }

    public class SomeTask
    {
        private static Dictionary<string, string> args = new Dictionary<string, string>()
        {
            { "-p", "Path from arg" },
        };

        public string Path { get; set; }

        public SomeTask AddPath(string path)
        {
            Path = path;
            return this;
        }

        public void AddArgMapping(Expression<Action<SomeTask>> taskMethod, string key, string help = null)
        {
            if (!args.ContainsKey(key))
            {
               taskMethod.Compile().Invoke(this);
               return; 
            }

            string value = args[key];
            MethodParameterModifier parameterModifier = new MethodParameterModifier();
            var newExpression = (Expression<Action<SomeTask>>) parameterModifier.Modify(taskMethod, value);
            var action = newExpression.Compile();
            action.Invoke(this);
        }
    }

    public class MethodParameterModifier : ExpressionVisitor
    {
        private string _value;

        public Expression Modify(Expression expression, string value)
        {
            _value = value;
            return Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            List<ConstantExpression> newargs = new List<ConstantExpression>();
            newargs.Add(Expression.Constant(_value, typeof(string)));
            MethodCallExpression methodCallExpression = node.Update(node.Object, newargs);
            return base.VisitMethodCall(methodCallExpression);
        }
    }
}
