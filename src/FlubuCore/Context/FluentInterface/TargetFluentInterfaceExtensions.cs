using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Targeting;

namespace FlubuCore.Context.FluentInterface
{
    public static class TargetFluentInterfaceExtensions
    {
        public static ITarget DependenceOf(this ITarget target, params string[] targetNames)
        {
            var t = target as TargetFluentInterface;
            t.Target.DependenceOf(targetNames);
            return target;
        }

        public static ITarget DependenceOfAsync(this ITarget target, params string[] targetNames)
        {
            var t = target as TargetFluentInterface;
            t.Target.DependenceOfAsync(targetNames);
            return target;
        }

        public static ITarget DependenceOf(this ITarget target, params ITarget[] targets)
        {
            var t = target as TargetFluentInterface;
            foreach (var td in targets)
            {
                var ret = td as TargetFluentInterface;
                t.Target.DependenceOf(ret.Target);
            }

            return target;
        }

        public static ITarget DependenceOfAsync(this ITarget target, params ITarget[] targets)
        {
            var t = target as TargetFluentInterface;
            foreach (var td in targets)
            {
                var ret = td as TargetFluentInterface;
                t.Target.DependenceOfAsync(ret.Target);
            }

            return target;
        }
    }
}
