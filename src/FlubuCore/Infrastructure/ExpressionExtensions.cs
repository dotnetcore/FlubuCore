using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FlubuCore.Infrastructure
{
    public static class ExpressionExtensions
    {
        public static object GetValue(this MemberInfo member, object obj)
        {
            if (member is PropertyInfo p)
            {
                return p.GetValue(obj);
            }
            else if (member is FieldInfo f)
            {
                return f.GetValue(obj);
            }

            throw new NotSupportedException();
        }
    }
}
