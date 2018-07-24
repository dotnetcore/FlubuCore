using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    public interface ITaskSessionFactory
    {
        ITaskSession OpenTaskSession();
    }
}
