using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Repository.Exceptions
{
    public class NotUniqueException : Exception
    {
        public NotUniqueException(string message)
            : base(message)
        {
        }
    }
}
