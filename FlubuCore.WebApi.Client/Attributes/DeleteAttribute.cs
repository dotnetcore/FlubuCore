using System.Net.Http;

namespace FlubuCore.WebApi.Client.Attributes
{
    public class DeleteAttribute : HttpAttribute
    {
        public DeleteAttribute(string path)
            : base(path)
        {
        }

        public override HttpMethod Method
        {
            get { return HttpMethod.Delete; }
        }
    }
}
