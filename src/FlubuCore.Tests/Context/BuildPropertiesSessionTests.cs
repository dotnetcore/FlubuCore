using System;
using System.Runtime.InteropServices.ComTypes;
using FlubuCore.Context;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Solution.VSSolutionBrowsing;
using FlubuCore.Tasks.Versioning;
using Xunit;

namespace FlubuCore.Tests.Context
{
    public class BuildPropertiesSessionTests
    {
        private IBuildPropertiesSession _session;

        public BuildPropertiesSessionTests()
        {
            _session = new BuildPropertiesSession(new TargetTree(null, null));
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

        [Fact]
        public void SetSolutionFileNameRightType()
        {
            _session.Set(BuildProps.SolutionFileName, "test");
        }

        [Fact]
        public void SetBuildConfigurationRightType()
        {
            _session.Set(BuildProps.BuildConfiguration, "test");
        }

        [Fact]
        public void SetProductIdRightType()
        {
            _session.Set(BuildProps.ProductId, "test");
        }

        [Fact]
        public void SetSolutionFileNameWrongType()
        {
           Assert.Throws<TaskValidationException>(() => _session.Set(BuildProps.SolutionFileName, 1));
        }

        [Fact]
        public void SetBuildVersionRightType()
        {
            _session.Set(BuildProps.BuildVersion, new BuildVersion());
        }

        [Fact]
        public void SetBuildVersionWrongType()
        {
            Assert.Throws<TaskValidationException>(() => _session.Set(BuildProps.SolutionFileName, new Version()));
        }

        [Fact]
        public void SetBuildConfigurationWrongType()
        {
            Assert.Throws<TaskValidationException>(() => _session.Set(BuildProps.BuildConfiguration, 1));
        }

        [Fact]
        public void SetProductIdWrongType()
        {
            Assert.Throws<TaskValidationException>(() => _session.Set(BuildProps.ProductId, 1));
        }

        [Fact]
        public void SetProductRootDirWrongType()
        {
            Assert.Throws<TaskValidationException>(() => _session.Set(BuildProps.ProductRootDir, 1));
        }
    }
}
