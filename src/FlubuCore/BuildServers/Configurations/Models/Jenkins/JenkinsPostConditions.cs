using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public enum JenkinsPostConditions
    {
        /// <summary>
        /// Run the steps in the post section regardless of the completion status of the Pipeline’s or stage’s run.
        /// </summary>
        Always,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run has a different completion status from its previous run.
        /// </summary>
        Changed,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run is successful and the previous run failed or was unstable.
        /// </summary>
        Fixed,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run’s status is failure, unstable, or aborted and the previous run was successful.
        /// </summary>
        Regression,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run has an "aborted" status, usually due to the Pipeline being manually aborted. This is typically denoted by gray in the web UI.
        /// </summary>
        Aborted,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run has a "failed" status, typically denoted by red in the web UI.
        /// </summary>
        Failure,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run has a "success" status, typically denoted by blue or green in the web UI.
        /// </summary>
        Success,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run has an "unstable" status, usually caused by test failures, code violations, etc. This is typically denoted by yellow in the web UI.
        /// </summary>
        Unstable,

        /// <summary>
        /// Only run the steps in post if the current Pipeline’s or stage’s run has not a "success" status. This is typically denoted in the web UI depending on the status previously mentioned.
        /// </summary>
        Unsuccesfull,

        /// <summary>
        /// Run the steps in this post condition after every other post condition has been evaluated, regardless of the Pipeline or stage’s status.
        /// </summary>
        Cleanup
    }
}
