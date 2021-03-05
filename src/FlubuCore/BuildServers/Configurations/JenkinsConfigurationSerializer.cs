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
        public string Serialize(JenkinsPipeline pipeline)
        {
            return BuildJenkinsPipeline(pipeline).ToString();
        }

        private StringBuilder BuildJenkinsPipeline(JenkinsPipeline pipeline)
        {
            StringBuilder stringBuilder = new StringBuilder();

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
                foreach (var stage in stages)
                {
                    AppendStage(sb, stage);
                }
            }, 4);
        }

        private void AppendStage(StringBuilder sb, Stage stage)
        {
            sb.AppendBlockWithNewLine($"stage('{stage.Name}')", (s) =>
            {
                sb.AppendBlock("steps", (s2) =>
                {
                    if (!string.IsNullOrEmpty(stage.WorkingDirectory))
                    {
                        s.AppendBlock($"dir('{stage.WorkingDirectory}')", s3 =>
                        {
                            foreach (var stageStep in stage.Steps)
                            {
                                s2.AppendIndent(20).AppendLine(stageStep);
                            }
                        }, 16);
                    }
                    else
                    {
                        foreach (var stageStep in stage.Steps)
                        {
                            s.AppendIndent(16).AppendLine(stageStep);
                        }
                    }
                }, 12);
            }, 8);
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
                return;
            }

            sb.AppendBlockWithNewLine("options", (s) =>
            {
                var indent = new string(' ', 8);
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
    }
}
