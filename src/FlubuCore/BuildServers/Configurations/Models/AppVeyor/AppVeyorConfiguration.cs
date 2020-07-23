using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Infrastructure;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.AppVeyor
{
    public class AppVeyorConfiguration
    {
        public List<string> Image { get; set; }

        [YamlMember(Alias = "clone_depth", ApplyNamingConventions = false)]
        public int? CloneDepth { get; set; }

        [YamlMember(Alias = "skip_commits", ApplyNamingConventions = false)]
        public SkipCommits SkipCommits { get; set; }

        public List<string> Services { get; set; }

        public List<For> For { get; set; }

        public void FromOptions(AppVeyorOptions options)
        {
            CloneDepth = options.CloneDepth;

            if (options.Images.IsNullOrEmpty())
            {
                options.SetVirtualMachineImage(AppVeyorImage.VisualStudio2019, AppVeyorImage.Ubuntu1804);
            }

            Image = options.Images;

            if (!options.SkipCommitsFiles.IsNullOrEmpty())
            {
                SkipCommits = new SkipCommits
                {
                    Files = options.SkipCommitsFiles
                };
            }

            if (!options.Services.IsNullOrEmpty())
            {
                Services = options.Services;
            }

            For = new List<For>();

            foreach (var image in options.Images)
            {
                For f = new For();

                var matrix = new Matrix();
                matrix.Only = new List<Only>
                {
                    new Only { Image = image }
                };

                f.BeforeBuild.Add(CreateBuild("dotnet tool install --global FlubuCore.Tool --version 5.1.8", image));

                foreach (var beforeBuild in options.BeforeBuilds)
                {
                    if (beforeBuild.image.Equals("all", StringComparison.OrdinalIgnoreCase) || beforeBuild.image == image)
                    {
                        f.BeforeBuild.Add(CreateBuild(beforeBuild.beforeScript, beforeBuild.image));
                    }
                }

                foreach (var script in options.CustomScriptsBeforeTarget)
                {
                    if (script.image.Equals("all", StringComparison.OrdinalIgnoreCase) || script.image == image)
                    {
                        f.BuildScript = new List<Build>();
                        f.BuildScript.Add(CreateBuild(script.script, script.image));
                    }
                }

                bool customTargetAddedForImage = false;

                foreach (var target in options.Targets)
                {
                    if (target.image.Equals("all", StringComparison.OrdinalIgnoreCase) || target.image == image)
                    {
                        f.BuildScript.Add(CreateBuild($"flubu {target.target}", target.image));
                        customTargetAddedForImage = true;
                    }
                }

                if (!customTargetAddedForImage)
                {
                    f.BuildScript.Add(CreateBuild($"flubu {string.Join(" ", options.TargetNames)}", image));
                }

                foreach (var script in options.CustomScriptsAfterTarget)
                {
                    if (script.image.Equals("all", StringComparison.OrdinalIgnoreCase) || script.image == image)
                    {
                        f.BuildScript = new List<Build>();
                        f.BuildScript.Add(CreateBuild(script.script, script.image));
                    }
                }

                f.Matrix = matrix;
                For.Add(f);
            }
        }

        private Build CreateBuild(string script, string image)
        {
            var build = new Build();
            if (image.StartsWith("Visual"))
            {
                build.PowerShell = script;
            }
            else
            {
                build.Shell = script;
            }

            return build;
        }
    }
}
