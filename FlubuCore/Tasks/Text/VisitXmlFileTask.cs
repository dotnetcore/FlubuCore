using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Xml;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    /// <summary>
    /// Runs XPath queries on the specified XML file and provides an interface for visiting each query result.
    /// </summary>
    public class VisitXmlFileTask : TaskBase<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisitXmlFileTask"/> class with the specified
        /// XML file to be analyzed. 
        /// </summary>
        /// <param name="xmlFileName">
        /// File name of the XML file to be queried.
        /// </param>
        public VisitXmlFileTask(string xmlFileName)
        {
            this.xmlFileName = xmlFileName;
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

            visitors.Add(new Visitor(xpath, visitorFunc));
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);

            PerformVisits(context, xmlDoc);

            return 0;
        }

        private void PerformVisits(ITaskContextInternal context, XmlDocument xmlDoc)
        {
            foreach (Visitor visitor in visitors)
                visitor.PerformVisit(context, xmlDoc);
        }

        private readonly string xmlFileName;
        private readonly List<Visitor> visitors = new List<Visitor>();

        private class Visitor
        {
            public Visitor(string xpath, Func<XmlNode, bool> visitorFunc)
            {
                Contract.Requires(xpath != null);
                Contract.Requires(visitorFunc != null);

                this.xpath = xpath;
                this.visitorFunc = visitorFunc;
            }

            public void PerformVisit(ITaskContextInternal context, XmlDocument xmlDoc)
            {
                context.LogInfo($"Performing visit on XPath '{xpath}'");

                XmlNodeList nodes = xmlDoc.SelectNodes(xpath);
                if (nodes == null || nodes.Count == 0)
                {
                    context.LogInfo($"XPath '{xpath}' returns empty list");
                    return;
                }

                context.LogInfo($"XPath '{xpath}' returns {nodes.Count} nodes");
                foreach (XmlNode node in nodes)
                {
                    if (!visitorFunc(node))
                        return;
                }
            }

            private readonly string xpath;
            private readonly Func<XmlNode, bool> visitorFunc;
        }
    }
}