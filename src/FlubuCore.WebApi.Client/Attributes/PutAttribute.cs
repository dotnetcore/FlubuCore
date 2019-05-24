using System.Net.Http;

namespace FlubuCore.WebApi.Client.Attributes
{
    public class PutAttribute : HttpAttribute
    {
        public PutAttribute(string path)
            : base(path)
        {
        }

        public override HttpMethod Method
        {
            get { return HttpMethod.Put; }
        }
    }
}
