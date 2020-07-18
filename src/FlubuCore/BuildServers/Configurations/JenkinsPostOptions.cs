using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlubuCore.BuildServers.Configurations.Models.Jenkins;

namespace FlubuCore.BuildServers.Configurations
{
    public class JenkinsPostOptions
    {
        private JenkinsOptions _options;

        public JenkinsPostOptions(JenkinsOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Adds custom post step on specified condition.
        /// </summary>
        /// <param name="condition">The condition when step's will be executed.</param>
        /// <param name="customStep">Step to execute.</param>
        /// <returns></returns>
        public JenkinsPostOptions AddCustomPostStep(JenkinsPostConditions condition, string customStep)
        {
            var jenkinsPost = _options.JenkinsPosts.FirstOrDefault(x => x.Condition == condition);

            if (jenkinsPost == null)
            {
                jenkinsPost = new JenkinsPost()
                {
                    Condition = condition,
                };

                jenkinsPost.Steps.Add(customStep);
                _options.JenkinsPosts.Add(jenkinsPost);
                return this;
            }

            jenkinsPost.Steps.Add(customStep);
            return this;
        }
    }
}
