using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Templating.Models;
using Newtonsoft.Json;
using Xunit;

namespace FlubuCore.Tests.Templating
{
    public class TemplateReplacementTokenModelTests
    {
        [Fact]
        public void NoInputType_InputTypeShouldBeText()
        {
            string json = @"{
	""Tokens"": [
		{
			""Token"": ""{{SolutionFileName}}"",
			""Description"": ""Enter relative path to solution filename:"",			
		}
	]
}";
            var model = JsonConvert.DeserializeObject<TemplateModel>(json);
           Assert.Equal(InputType.Text, model.Tokens[0].InputType);
        }
    }
}
