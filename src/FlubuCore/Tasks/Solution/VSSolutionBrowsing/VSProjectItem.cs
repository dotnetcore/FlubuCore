using System.Collections.Generic;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// Holds information about content items inside of a VisualStudio project.
    /// </summary>
    public class VSProjectItem
    {
        public const string Content = "Content";

        public const string CompileItem = "CompileItem";

        public const string NoneItem = "None";

        public const string ProjectReference = "ProjectReference";

        public const string Reference = "Reference";

        private readonly Dictionary<string, string> _itemProperties = new Dictionary<string, string>();

        public VSProjectItem(string itemType)
        {
            ItemType = itemType;
        }

        public string Item { get; set; }

        public string ItemType { get; }

        public IDictionary<string, string> ItemProperties => _itemProperties;
    }
}