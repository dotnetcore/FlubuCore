using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.AppVeyor
{
    public class For
    {
        public Matrix Matrix { get; set; } = new Matrix();

        [YamlMember(Alias = "before_build", ApplyNamingConventions = false)]
        public List<Build> BeforeBuild { get; set; } = new List<Build>();

        [YamlMember(Alias = "build_script", ApplyNamingConventions = false)]
        public List<Build> BuildScript { get; set; } = new List<Build>();
    }
}
