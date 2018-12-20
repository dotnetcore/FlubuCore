using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.WebApi.Model;

namespace FlubuCore.WebApi.Client
{
    public class Response<T>
    {
        public T Data { get; set; }

        public ErrorModel Error { get; set; }
    }
}
