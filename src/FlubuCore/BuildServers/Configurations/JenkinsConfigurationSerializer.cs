using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FlubuCore.BuildServers.Configurations.Models.Jenkins;

namespace FlubuCore.BuildServers.Configurations
{
    public class JenkinsConfigurationSerializer
    {
        private JenkinsPipelineOptions _jenkinsPipelineOptions;

        public string Serialize(JenkinsPipeline pipeline)
        {
            _jenkinsPipelineOptions = pipeline.JenkinsPipelineOptions;
            return BuildJenkinsPipeline(pipeline).ToString();
        }

        private StringBuilder BuildJenkinsPipeline(JenkinsPipeline pipeline)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!pipeline.JenkinsPipelineOptions.DisableCustomWorkspaceFlubuFeature)
            {
                stringBuilder.AppendBlockWithNewLine("def getWorkspace()", sb =>
                {
                    sb.AppendIndent(4).Append(@"pwd().replace(""%2F"", ""_"")");
                });
            }

            stringBuilder.AppendBlock("pipeline", sb =>
            {
                sb.Append(new string(' ', 4)).Append("agent ").AppendLine(pipeline.Agent).AppendLine();
                AppendOptions(sb, pipeline.Options);
                AppendEnvironment(sb, pipeline.Environment);
                AppendStages(sb, pipeline.Stages);
                AppendPost(sb, pipeline.Post);
            });

            return stringBuilder;
        }

        private void AppendEnvironment(StringBuilder sb, Dictionary<string, string> environment)
        {
            if (environment == null || environment.Count == 0)
            {
                return;
            }

            sb.AppendBlockWithNewLine("environment", (s) =>
            {
                foreach (var entry in environment)
                {
                    sb.AppendIndent(8).AppendFormat("{0} = {1}", entry.Key, entry.Value).AppendLine();
                }
            }, 4);
        }

        private void AppendStages(StringBuilder sb, List<Stage> stages)
        {
            if (stages == null || stages.Count == 0)
            {
                return;
            }

            sb.AppendBlockWithNewLine("stages", (s) =>
            {
                if (!_jenkinsPipelineOptions.DisableCustomWorkspaceFlubuFeature)
                {
                    AddCustomCheckoutStage(s);
                }

                foreach (var stage in stages)
                {
                    AppendFlubuStage(sb, stage);
                }
            }, 4);
        }

        private void AppendFlubuStage(StringBuilder sb, Stage stage)
        {
            sb.AppendBlockWithNewLine($"stage('{stage.Name}')", (s) =>
            {
                s.AppendBlock("steps", (s2) =>
                {
                    if (!_jenkinsPipelineOptions.DisableCustomWorkspaceFlubuFeature)
                    {
                        s2.AppendBlock("ws(getWorkspace())", (s3) =>
                        {
                            AppendStageStepsWithWorkingDirectory(stage, s3, 4);
                        }, 16);
                    }
                    else
                    {
                        AppendStageStepsWithWorkingDirectory(stage, s2, 0);
                    }
                }, 12);
            }, 8);
        }

        private void AppendStageStepsWithWorkingDirectory(Stage stage, StringBuilder s, int increaseIndent)
        {
            if (!string.IsNullOrEmpty(stage.WorkingDirectory))
            {
                s.AppendBlock($"dir('{stage.WorkingDirectory}')", s3 =>
                {
                    AppendStageSteps(stage, s, 20 + increaseIndent);
                }, 16 + increaseIndent);
            }
            else
            {
                AppendStageSteps(stage, s, 16 + increaseIndent);
            }
        }

        private void AppendStageSteps(Stage stage, StringBuilder s, int indent)
        {
            foreach (var stageStep in stage.Steps)
            {
                s.AppendIndent(indent).AppendLine(stageStep);
            }
        }

        private void AppendPost(StringBuilder sb, List<JenkinsPost> posts)
        {
            if (posts == null || posts.Count == 0)
            {
                return;
            }

            sb.AppendBlock("post", s =>
            {
                foreach (var post in posts)
                {
                    sb.AppendBlock(post.Condition.ToString().ToLower(), s2 =>
                    {
                        foreach (var postStep in post.Steps)
                        {
                            sb.AppendIndent(12).AppendLine(postStep);
                        }
                    }, 8);
                }
            }, 4);
        }

        private void AppendOptions(StringBuilder sb, JenkinsOptionsDirective options)
        {
            if (options == null)
            {
                if (!_jenkinsPipelineOptions.DisableCustomWorkspaceFlubuFeature)
                {
                    sb.AppendBlockWithNewLine("options", (s) =>
                    {
                        var indent = new string(' ', 8);

                        sb.Append(indent).AppendLine("skipDefaultCheckout()");
                    }, 4);
                }

                return;
            }

            sb.AppendBlockWithNewLine("options", (s) =>
            {
                var indent = new string(' ', 8);

                if (!_jenkinsPipelineOptions.DisableCustomWorkspaceFlubuFeature)
                {
                    sb.Append(indent).AppendLine("skipDefaultCheckout()");
                }

                if (options.DisableConcurrentBuilds)
                {
                    sb.Append(indent).AppendLine("disableConcurrentBuilds()");
                }

                if (options.DisableResume)
                {
                    sb.Append(indent).AppendLine("disableResume()");
                }

                if (options.SkipDefaultCheckout)
                {
                    sb.Append(indent).AppendLine("skipDefaultCheckout()");
                }

                if (options.SkipStagesAfterUnstable)
                {
                    sb.Append(indent).AppendLine("skipStagesAfterUnstable()");
                }

                if (options.TimeStamps)
                {
                    sb.Append(indent).AppendLine("timestamps()");
                }

                if (options.AnsiColors)
                {
                    sb.Append(indent).AppendLine("ansiColor('xterm')");
                }

                if (options.BuildDiscarder.HasValue)
                {
                    sb.Append(indent).AppendFormat("buildDiscarder(logRotator(numToKeepStr: '{0}')) }", options.BuildDiscarder.Value);
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(options.CheckoutToSubDirectory))
                {
                    sb.Append(indent).AppendFormat("checkoutToSubdirectory('{0}')", options.CheckoutToSubDirectory);
                    sb.AppendLine();
                }

                if (options.PreserveStashes)
                {
                    if (options.PreserveStashesBuildCount.HasValue)
                    {
                        sb.Append(indent).AppendFormat("preserveStashes(buildCount: {0})", options.PreserveStashesBuildCount).AppendLine();
                    }
                    else
                    {
                        sb.Append(indent).AppendLine("preserveStashes()");
                    }
                }

                if (options.QuietPeriod.HasValue)
                {
                    sb.Append(indent).AppendFormat("quietPeriod({0})", options.QuietPeriod);
                    sb.AppendLine();
                }

                if (options.Retry.HasValue)
                {
                    sb.Append(indent).AppendFormat("retry({0})", options.Retry);
                    sb.AppendLine();
                }

                if (options.Timeout.HasValue)
                {
                    sb.Append(indent).AppendFormat("timeout(time: {0}, unit: '{1}')", options.Timeout, options.TimeoutUnit);
                    sb.AppendLine();
                }
            }, 4);
        }

        private void AddCustomCheckoutStage(StringBuilder s)
        {
            s.AppendBlockWithNewLine($"stage('Checkout')", (s2) =>
            {
                s2.AppendBlock("steps", (s3) =>
                {
                    s3.AppendBlock("ws(getWorkspace())", (s4) =>
                    {
                        s4.AppendIndent(20).AppendLine("checkout([");
                        s4.AppendIndent(24).AppendLine("$class: 'GitSCM', ");
                        s4.AppendIndent(24).AppendLine("branches: scm.branches,");
                        s4.AppendIndent(24).AppendLine("doGenerateSubmoduleConfigurations: false,");
                        s4.AppendIndent(24).AppendLine("extensions: [[");
                        s4.AppendIndent(28).AppendLine("$class: 'SubmoduleOption',");
                        s4.AppendIndent(28).AppendLine("disableSubmodules: false,");
                        s4.AppendIndent(28).AppendLine("parentCredentials: true,");
                        s4.AppendIndent(28).AppendLine("recursiveSubmodules: true,");
                        s4.AppendIndent(28).AppendLine("reference: '',");
                        s4.AppendIndent(28).AppendLine("trackingSubmodules: false");
                        s4.AppendIndent(24).AppendLine("]],");
                        s4.AppendIndent(24).AppendLine("submoduleCfg: [],");
                        s4.AppendIndent(24).AppendLine("userRemoteConfigs: scm.userRemoteConfigs");
                        s4.AppendIndent(20).AppendLine("])");
                    }, 16);
                }, 12);
            }, 8);
        }
    }
}
