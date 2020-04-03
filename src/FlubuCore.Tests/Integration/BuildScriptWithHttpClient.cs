using System;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore.Tests.Integration
{
    public class BuildScriptWithHttpClient : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            session.CreateTarget("HttpClient").Do(HttpClientTest);
        }

        public void HttpClientTest(ITaskContext context)
        {
            var client = context.Tasks().CreateHttpClient("http://www.google.com");
            client.Timeout = new TimeSpan(0, 0, 10, 1);
        }
    }
}