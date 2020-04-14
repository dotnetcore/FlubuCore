using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using FlubuCore.Tasks;

namespace FlubuCore.Targeting
{
    /// <summary>
    /// This class is used to strore ordered dependencies of a target.
    /// </summary>
    public class TargetDependencyCollection : KeyedCollection<string, TargetDependency>
    {
        protected override string GetKeyForItem(TargetDependency item) => item.TargetName;

        public void Add(string dependentTargetName, TaskExecutionMode taskExecMode, bool skipped = false)
        {
            Add(new TargetDependency
            {
                TargetName = dependentTargetName,
                TaskExecutionMode = taskExecMode,
                Skipped = skipped
            });
        }

        public bool ContainsKey(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        public IEnumerable<string> GetKeys()
        {
            foreach (var item in Items)
            {
                yield return item.TargetName;
            }
        }
    }
}
