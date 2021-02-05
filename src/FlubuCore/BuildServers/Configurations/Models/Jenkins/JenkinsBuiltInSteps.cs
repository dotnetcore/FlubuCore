using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public static class JenkinsBuiltInSteps
    {
        public const string CheckoutStep = @"checkout([
            $class: 'GitSCM', 
            branches: scm.branches, 
            doGenerateSubmoduleConfigurations: false, 
            extensions: [[
            $class: 'SubmoduleOption', 
                disableSubmodules: false, 
                parentCredentials: true, 
                recursiveSubmodules: true, 
                reference: '', 
                trackingSubmodules: false            
            ], 
            [$class: 'CleanBeforeCheckout'], [$class: 'CleanCheckout']], 
			submoduleCfg: [], 
            userRemoteConfigs: scm.userRemoteConfigs
            ])";
    }
}
