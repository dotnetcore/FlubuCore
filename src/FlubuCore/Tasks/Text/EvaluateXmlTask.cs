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
        private readonly string _xmlFileName;
        private readonly List<Expression> _expressions = new List<Expression>();

        public EvaluateXmlTask(string xmlFileName)
        {
            _xmlFileName = xmlFileName;
        }

        protected override string Description { get; set; }

        public EvaluateXmlTask AddExpression(string propertyName, string xpath)
        {
            _expressions.Add(new Expression(propertyName, xpath));
            return this;
        }

        /// <summary>
        /// Internal task execution code.
        /// </summary>
        /// <param name="context">The script execution environment.</param>
        protected override int DoExecute(ITaskContextInternal context)
        {
            using (FileStream fileStream = new FileStream(_xmlFileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    XPathDocument doc = new XPathDocument(reader);
                    XPathNavigator navigator = doc.CreateNavigator();

                    foreach (Expression expression in _expressions)
                    {
                        object result = navigator.Evaluate(expression.Xpath);
                        context.Properties.Set(expression.PropertyName, result);

                        DoLogInfo(
                            $"Property '{expression.Xpath}': executing XPath expression '{expression.PropertyName}' evaluates to '{result}'");
                    }
                }
            }

            return 0;
        }

        private class Expression
        {
            public Expression(string propertyName, string xpath)
            {
                Xpath = xpath;
                PropertyName = propertyName;
            }

            public string Xpath { get; }

            public string PropertyName { get; }
        }
    }
}