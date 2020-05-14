using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Versioning;
using Xunit;

namespace FlubuCore.Tests.Tasks
{
    public class BuildVersionTests
    {
        [Theory]
        [InlineData(1, 0, 3, 2, null, "1.0.3.2")]
        [InlineData(1, 0, 3, 2, "", "1.0.3.2")]
        [InlineData(1, 0, 3, 2, "preview1", "1.0.3.2-preview1")]
        [InlineData(1, 0, 3, 2, "-preview1", "1.0.3.2-preview1")]
        public void BuildVersionWithQualityTests(int major, int minor, int build, int revision, string quality, string expextedBuildVersionWithQuality)
        {
            BuildVersion version = new BuildVersion();
            version.Version = new Version(major, minor, build, revision);
            version.VersionQuality = quality;

            Assert.Equal(expextedBuildVersionWithQuality, version.BuildVersionWithQuality());
        }

        [Theory]
        [InlineData(1, 0, 3, 2, null, "1.0.3")]
        [InlineData(1, 0, 3, 2, "", "1.0.3")]
        [InlineData(1, 0, 3, 2, "preview1", "1.0.3-preview1")]
        [InlineData(1, 0, 3, 2, "-preview1", "1.0.3-preview1")]
        public void BuildVersionWithQualityVersionFieldCountTests(int major, int minor, int build, int revision, string quality, string expextedBuildVersionWithQuality)
        {
            BuildVersion version = new BuildVersion();
            version.Version = new Version(major, minor, build, revision);
            version.VersionQuality = quality;

            Assert.Equal(expextedBuildVersionWithQuality, version.BuildVersionWithQuality(3));
        }
    }
}
