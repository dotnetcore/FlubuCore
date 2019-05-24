using System;

namespace FlubuCore.Scripting.Attributes.Config
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DisableLoadScriptReferencesAutomaticallyAttribute : Attribute
    {
    }
}
