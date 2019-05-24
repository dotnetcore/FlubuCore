using System.Net.Http;

namespace FlubuCore.WebApi.Client.Attributes
{
    public class GetAttribute : HttpAttribute
    {
        public GetAttribute(string path)
            : base(path)
        {
        }

        public override HttpMethod Method
        {
            get { return HttpMethod.Get; }
        }
    }
}
