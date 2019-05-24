using FlubuCore.Context;
using Xunit;

namespace FlubuCore.Tests.Context
{
    public class BuildPropertiesSessionTests
    {
        private IBuildPropertiesSession _session;

        public BuildPropertiesSessionTests()
        {
            _session = new BuildPropertiesSession();
        }

        [Fact]
        public void Get_CaseInsensitiveGet_Succesfull()
        {
            _session.Set("SomeKey", "Value");
            Assert.Equal("Value", _session.Get<string>("SOMEKEY"));
        }

        [Fact]
        public void TryGet_CaseInsensitiveTryGet_Succesfull()
        {
            _session.Set("SomeKey", "Value");
            Assert.Equal("Value", _session.TryGet<string>("SoMEKEY"));
        }

        [Fact]
        public void Remove_CaseInsensitiveTryGet_Succesfull()
        {
            _session.Set("somekey", "Value");
            _session.Remove("SOMEKEY");
            Assert.Null(_session.TryGet<string>("somekey"));
        }
    }
}
