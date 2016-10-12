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

        private readonly string _itemType;

        private string _item;

        public VSProjectItem(string itemType)
        {
            _itemType = itemType;
        }

        public string Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public string ItemType
        {
            get { return _itemType; }
        }

        public IDictionary<string, string> ItemProperties
        {
            get { return _itemProperties; }
        }
    }
}