using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using FlubuCore;
using Xunit;

namespace Flubu.Tests
{
    public class Tests
    {
        [Fact]
        public void StringValueFromArgumentTest()
        {
            SomeTask task = new SomeTask();
            task.FromArgument(x => x.AddPath("default path"), "-p", "help bla bla");
            Assert.Equal("Path from arg", task.Path);
        }

        [Fact]
        public void IntValueFromArgumentTest()
        {
            SomeTask task = new SomeTask();
            task.FromArgument(x => x.SetLevel(0), "-l", "help bla bla");
            Assert.Equal(2, task.Level);
        }

        [Fact]
        public void DefaultValuetTest()
        {
            SomeTask task = new SomeTask();
            task.FromArgument(x => x.AddPath("default path"), "-nonexist");
            Assert.Equal("default path", task.Path);
        }
    }

    public class SomeTask
    {
        private static Dictionary<string, string> args = new Dictionary<string, string>()
        {
            { "-p", "Path from arg" },
            { "-l", "2" },
        };

        public string Path { get; set; }

        public int Level { get; set; }

        public SomeTask AddPath(string path)
        {
            Path = path;
            return this;
        }

        public SomeTask SetLevel(int level)
        {
            Level = level;
            return this;
        }

        public void FromArgument(Expression<Action<SomeTask>> taskMethod, string key, string help = null)
        {
            if (!args.ContainsKey(key))
            {
               taskMethod.Compile().Invoke(this);
               return; 
            }

            string value = args[key];
            MethodParameterModifier parameterModifier = new MethodParameterModifier();
            var newExpression = (Expression<Action<SomeTask>>) parameterModifier.Modify(taskMethod, new List<string>() { value });
            var action = newExpression.Compile();
            action.Invoke(this);
        }
    }
}
