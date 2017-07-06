using System;
using System.Net.Http;

namespace FlubuCore.WebApi.Client.Attributes
{
    public abstract class HttpAttribute : Attribute
    {
        protected HttpAttribute(string path)
        {
            Path = path;
        }

        public abstract HttpMethod Method { get; }

        public string Path { get; protected set; }
    }
}
