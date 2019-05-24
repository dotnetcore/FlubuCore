using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    /// <summary>
    ///     Updates an XML file using the specified update commands.
    /// </summary>
    public class UpdateXmlFileTask : TaskBase<int, UpdateXmlFileTask>
    {
        private readonly string _fileName;
        private readonly Dictionary<string, string> _xpathAddOrUpdates = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _xpathUpdates = new Dictionary<string, string>();
        private readonly List<string> _xpathDeletes = new List<string>();
        private readonly List<UpdateXmlFileTaskAddition> _xpathAdditions = new List<UpdateXmlFileTaskAddition>();
        private string _description;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateXmlFileTask" /> class with
        ///     the specified XML file to be updated.
        /// </summary>
        /// <param name="fileName">The fileName of the XML file.</param>
        public UpdateXmlFileTask(string fileName)
        {
            _fileName = fileName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return $"Update XML file '{_fileName}'";

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        ///     Adds an "update" command to the list of commands to be performed on the XML file.
        /// </summary>
        /// <param name="xpath">XPath for the nodes which should be updated.</param>
        /// <param name="value">New value of the selected nodes.</param>
        public UpdateXmlFileTask UpdatePath(string xpath, string value)
        {
            _xpathUpdates.Add(xpath, value);
            return this;
        }

        /// <summary>
        ///     Adds an "delete" command to the list of commands to be performed on the XML file.
        /// </summary>
        /// <param name="xpath">XPath for the nodes which should be deleted.</param>
        public UpdateXmlFileTask DeletePath(string xpath)
        {
            _xpathDeletes.Add(xpath);
            return this;
        }

        /// <summary>
        ///     Adds an "add or update" command to the list of commands to be performed on the XML file. Depending on if the xml
        ///     element exists or not.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="value"></param>
        public UpdateXmlFileTask AddOrUpdate(string xpath, string value)
        {
            _xpathAddOrUpdates.Add(xpath, value);
            return this;
        }

        /// <summary>
        ///     Adds an "add" command to the list of commands to be performed on the XML file.
        /// </summary>
        /// <param name="rootXpath">XPath for the root node on which an addition should be performed.</param>
        /// <param name="childNodeName">Name of the new child node.</param>
        /// <param name="value">The value for the new child node.</param>
        public UpdateXmlFileTask AddPath(string rootXpath, string childNodeName, string value)
        {
            var addition = new UpdateXmlFileTaskAddition(rootXpath, childNodeName, value);
            _xpathAdditions.Add(addition);
            return this;
        }

        /// <summary>
        ///     Adds an "add" command to the list of commands to be performed on the XML file.
        /// </summary>
        /// <param name="rootXpath">XPath for the root node on which an addition should be performed.</param>
        /// <param name="childNodeName">Name of the new child node.</param>
        public UpdateXmlFileTask AddPath(string rootXpath, string childNodeName)
        {
            var addition = new UpdateXmlFileTaskAddition(rootXpath, childNodeName);
            _xpathAdditions.Add(addition);
            return this;
        }

        /// <summary>
        ///     Adds an "add" command to the list of commands to be performed on the XML file.
        /// </summary>
        /// <param name="rootXpath">XPath for the root node on which an addition should be performed.</param>
        /// <param name="childNodeName">Name of the new child node.</param>
        /// <param name="attributes">Attributes to be added.</param>
        public UpdateXmlFileTask AddPath(string rootXpath, string childNodeName, IDictionary<string, string> attributes)
        {
            var addition = new UpdateXmlFileTaskAddition(rootXpath, childNodeName, attributes);
            _xpathAdditions.Add(addition);
            return this;
        }

        /// <summary>
        ///     Method defining the actual work for a task.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        /// <returns></returns>
        protected override int DoExecute(ITaskContextInternal context)
        {
            using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                var xmlDoc = new XmlDocument();

                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(fs);
                AddOrUpdates(xmlDoc);
                PerformDeletes(xmlDoc, context);
                PerformUpdates(xmlDoc, context);
                PerformAdditions(xmlDoc, context);
                fs.SetLength(0);
                xmlDoc.Save(fs);
            }

            return 0;
        }

        private void AddOrUpdates(XmlDocument xmldoc)
        {
            foreach (var keyValue in _xpathAddOrUpdates)
            {
                if (xmldoc.SelectSingleNode(keyValue.Key) == null)
                {
                    var index = keyValue.Key.LastIndexOf("/");
                    var xpath = keyValue.Key.Substring(0, index);
                    var nodeNameToAdd = keyValue.Key.Substring(index + 1);
                    var addition = new UpdateXmlFileTaskAddition(xpath, nodeNameToAdd, keyValue.Value);
                    _xpathAdditions.Add(addition);
                }
                else
                {
                    _xpathUpdates.Add(keyValue.Key, keyValue.Value);
                }
            }
        }

        private void PerformDeletes(XmlDocument xmldoc, ITaskContextInternal context)
        {
            foreach (var xpath in _xpathDeletes)
            {
                foreach (XmlNode node in xmldoc.SelectNodes(xpath))
                {
                    var fullNodePath = ConstructXmlNodeFullName(node);

                    DoLogInfo($"Node '{fullNodePath}' will be removed.");

                    if (node.NodeType == XmlNodeType.Element)
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                    else if (node.NodeType == XmlNodeType.Attribute)
                    {
                        var attribute = (XmlAttribute)node;
                        attribute.OwnerElement.RemoveAttributeNode(attribute);
                    }
                    else
                    {
                        throw new ArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Node '{0}' is of incorrect type '{1}', it should be an element or attribute.",
                                fullNodePath,
                                node.NodeType));
                    }
                }
            }
        }

        private void PerformUpdates(XmlDocument xmldoc, ITaskContextInternal context)
        {
            foreach (var xpath in _xpathUpdates.Keys)
            {
                foreach (XmlNode node in xmldoc.SelectNodes(xpath))
                {
                    UpdateNode(xpath, node, context);
                }
            }
        }

        private void UpdateNode(string xpath, XmlNode node, ITaskContextInternal context)
        {
            var fullNodePath = ConstructXmlNodeFullName(node);

            DoLogInfo($"Node '{fullNodePath}' will have value '{_xpathUpdates[xpath]}'");

            if (node.NodeType == XmlNodeType.Attribute)
            {
                node.Value = _xpathUpdates[xpath];
            }
            else if (node.NodeType == XmlNodeType.Element)
            {
                node.InnerText = _xpathUpdates[xpath];
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Node '{0}' is of incorrect type '{1}', it should be an element or attribute.",
                        fullNodePath,
                        node.NodeType));
            }
        }

        private void PerformAdditions(XmlDocument xmldoc, ITaskContextInternal context)
        {
            foreach (var addition in _xpathAdditions)
            {
                var rootNode = xmldoc.SelectSingleNode(addition.RootXPath);

                if (rootNode == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Path '{0}' does not exist.",
                            addition.RootXPath));
                }

                if (rootNode.NodeType != XmlNodeType.Element)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Node '{0}' is of incorrect type '{1}', it should be an element.",
                            addition.RootXPath,
                            rootNode.NodeType));
                }

                XmlNode childNode = null;
                if (addition.ChildNodeName.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                {
                    childNode = xmldoc.CreateAttribute(addition.ChildNodeName.Substring(1));
                    childNode.Value = addition.Value;
                    rootNode.Attributes.Append((XmlAttribute)childNode);
                }
                else
                {
                    childNode = xmldoc.CreateElement(addition.ChildNodeName);

                    if (addition.Value != null)
                        childNode.InnerText = addition.Value;

                    if (addition.Attributes != null)
                    {
                        var element = (XmlElement)childNode;

                        foreach (var attribute in addition.Attributes.Keys)
                            element.SetAttribute(attribute, addition.Attributes[attribute]);
                    }

                    rootNode.AppendChild(childNode);
                }

                var fullNodePath = ConstructXmlNodeFullName(rootNode);

                DoLogInfo(
                    $"Node '{fullNodePath}' will have a new child '{childNode.Name}' with value '{addition.Value}'");
            }
        }

        private string ConstructXmlNodeFullName(XmlNode node)
        {
            var fullNodePath = new StringBuilder();
            string terminator = null;

            var node2 = node;
            while (node2 != null)
            {
                fullNodePath.Insert(0, terminator);
                fullNodePath.Insert(0, node2.Name);
                if (node2.NodeType == XmlNodeType.Attribute)
                    fullNodePath.Insert(0, '@');
                terminator = "/";

                if (node2.NodeType == XmlNodeType.Element)
                    node2 = node2.ParentNode;
                else if (node2.NodeType == XmlNodeType.Attribute)
                    node2 = ((XmlAttribute)node2).OwnerElement;
                else
                    node2 = null;
            }

            return fullNodePath.ToString();
        }

        internal class UpdateXmlFileTaskAddition
        {
            private readonly Dictionary<string, string> _attributes;

            internal UpdateXmlFileTaskAddition(string rootXPath, string childNodeName, string value)
            {
                RootXPath = rootXPath;
                ChildNodeName = childNodeName;
                Value = value;
            }

            internal UpdateXmlFileTaskAddition(string rootXPath, string childNodeName)
            {
                RootXPath = rootXPath;
                ChildNodeName = childNodeName;
            }

            internal UpdateXmlFileTaskAddition(string rootXPath, string childNodeName,
                IDictionary<string, string> attributes)
            {
                RootXPath = rootXPath;
                ChildNodeName = childNodeName;
                _attributes = new Dictionary<string, string>(attributes);
            }

            public string RootXPath { get; }

            public string ChildNodeName { get; }

            public string Value { get; }

            internal IDictionary<string, string> Attributes => _attributes;
        }
    }
}