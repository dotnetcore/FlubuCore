using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    /// <summary>
    /// Runs XPath queries on the specified XML file and provides an interface for visiting each query result.
    /// </summary>
    public class VisitXmlFileTask : TaskBase<int, VisitXmlFileTask>
    {
        private readonly string _xmlFileName;
        private readonly List<Visitor> _visitors = new List<Visitor>();
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitXmlFileTask"/> class with the specified
        /// XML file to be analyzed.
        /// </summary>
        /// <param name="xmlFileName">
        /// File name of the XML file to be queried.
        /// </param>
        public VisitXmlFileTask(string xmlFileName)
        {
            _xmlFileName = xmlFileName;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Update XML file '{_xmlFileName}";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Adds a visitor to be used for querying a specific XPath.
        /// </summary>
        /// <param name="xpath">XPath to be queried.</param>
        /// <param name="visitorFunc">The function that should be called on each XML node found by the query.</param>
        /// <returns>This same instance of the <see cref="VisitXmlFileTask"/>.</returns>
        public VisitXmlFileTask AddVisitor(string xpath, Func<XmlNode, bool> visitorFunc)
        {
            Contract.Requires(xpath != null);
            Contract.Requires(visitorFunc != null);
            Contract.Ensures(ReferenceEquals(Contract.Result<VisitXmlFileTask>(), this));

            _visitors.Add(new Visitor(xpath, visitorFunc));
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            using (FileStream fileStream = new FileStream(_xmlFileName, FileMode.Open, FileAccess.Read))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileStream);

                PerformVisits(context, xmlDoc);

                return 0;
            }
        }

        private void PerformVisits(ITaskContextInternal context, XmlDocument xmlDoc)
        {
            foreach (Visitor visitor in _visitors)
                visitor.PerformVisit(context, xmlDoc);
        }

        private class Visitor
        {
            private readonly string _xpath;
            private readonly Func<XmlNode, bool> _visitorFunc;

            public Visitor(string xpath, Func<XmlNode, bool> visitorFunc)
            {
                Contract.Requires(xpath != null);
                Contract.Requires(visitorFunc != null);

                _xpath = xpath;
                _visitorFunc = visitorFunc;
            }

            public void PerformVisit(ITaskContextInternal context, XmlDocument xmlDoc)
            {
                context.LogInfo($"Performing visit on XPath '{_xpath}'");

                XmlNodeList nodes = xmlDoc.SelectNodes(_xpath);
                if (nodes == null || nodes.Count == 0)
                {
                    context.LogInfo($"XPath '{_xpath}' returns empty list");
                    return;
                }

                context.LogInfo($"XPath '{_xpath}' returns {nodes.Count} nodes");
                foreach (XmlNode node in nodes)
                {
                    if (!_visitorFunc(node))
                        return;
                }
            }
        }
    }
}