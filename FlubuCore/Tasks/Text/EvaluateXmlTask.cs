using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Text
{
    /// <summary>
    /// Evaluates XPath expressions on a specified XML file and stores results in <seealso cref="ITaskContext"/> properties.
    /// </summary>
    public class EvaluateXmlTask : TaskBase<int, EvaluateXmlTask>
    {
        public EvaluateXmlTask(string xmlFileName)
        {
            this.xmlFileName = xmlFileName;
        }

        public EvaluateXmlTask AddExpression(string propertyName, string xpath)
        {
            expressions.Add(new Expression(propertyName, xpath));
            return this;
        }

        protected override string Description { get; set; }

        /// <summary>
        /// Internal task execution code.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        protected override int DoExecute(ITaskContextInternal context)
        {
            using (FileStream fileStream = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    XPathDocument doc = new XPathDocument(reader);
                    XPathNavigator navigator = doc.CreateNavigator();

                    foreach (Expression expression in expressions)
                    {
                        object result = navigator.Evaluate(expression.Xpath);
                        context.Properties.Set(expression.PropertyName, result);

                        context.LogInfo(
                            $"Property '{expression.Xpath}': executing XPath expression '{expression.PropertyName}' evaluates to '{result}'");
                    }
                }
            }

            return 0;
        }

        private readonly string xmlFileName;
        private readonly List<Expression> expressions = new List<Expression>();

        private class Expression
        {
            public Expression(string propertyName, string xpath)
            {
                this.xpath = xpath;
                this.propertyName = propertyName;
            }

            public string Xpath
            {
                get { return xpath; }
            }

            public string PropertyName
            {
                get { return propertyName; }
            }

            private readonly string xpath;
            private readonly string propertyName;
        }
    }
}