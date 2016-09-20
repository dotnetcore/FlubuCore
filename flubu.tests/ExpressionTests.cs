using flubu.Targeting;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;

namespace flubu.tests
{
    public static class ExpressionTests
    {
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;

                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member;
        }

        public static ITarget DependsOn<TProp>(this ITarget target, Expression<Func<TProp>> t)
        {
            return target.DependsOn(t.GetMemberInfo().Name);
        }

        public static void SampleCall()
        {
            ITarget t = new SimpleTarget();
            ITarget t1 = new SimpleTarget();
            t.DependsOn(() => t1);
        }
    }

    internal class SimpleTarget : ITarget
    {
        public ICollection<string> Dependencies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsHidden
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string TargetName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Stopwatch TaskStopwatch
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITarget DependsOn(params string[] targetNames)
        {
            throw new NotImplementedException();
        }

        public ITarget Do(Action<ITaskContext> targetAction)
        {
            throw new NotImplementedException();
        }

        public int Execute(ITaskContext context)
        {
            throw new NotImplementedException();
        }

        public ITarget OverrideDo(Action<ITaskContext> targetAction)
        {
            throw new NotImplementedException();
        }

        public ITarget SetAsDefault()
        {
            throw new NotImplementedException();
        }

        public ITarget SetAsHidden()
        {
            throw new NotImplementedException();
        }

        public ITarget SetDescription(string description)
        {
            throw new NotImplementedException();
        }
    }
}