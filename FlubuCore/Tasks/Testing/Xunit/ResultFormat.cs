using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Testing.Xunit
{
    public enum ResultFormat
    {
        /// <summary>
        /// output results to xUnit.net v2 XML file
        /// </summary>
        Xml,

        /// <summary>
        /// output results to xUnit.net v1 XML file
        /// </summary>
        XmlV1,

        /// <summary>
        /// output results to HTML file
        /// </summary>
        Html,

        /// <summary>
        /// output results to NUnit v2.5 XML file
        /// </summary>
        Nunit
    }
}
