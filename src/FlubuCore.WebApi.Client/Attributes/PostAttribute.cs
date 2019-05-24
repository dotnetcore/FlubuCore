using System.Net.Http;

namespace FlubuCore.WebApi.Client.Attributes
{
    public class PostAttribute : HttpAttribute
    {
        public PostAttribute(string path)
            : base(path)
        {
        }

        public override HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }
    }
}
