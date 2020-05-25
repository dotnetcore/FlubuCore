using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// Represents a VisualStudio project.
    /// </summary>
    public class VSProjectDetails
    {
        private readonly List<VSProjectConfiguration> _configurations = new List<VSProjectConfiguration>();

        private readonly List<VSProjectItem> _items = new List<VSProjectItem>();

        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        private bool _propertiesDictionary;

        /// <summary>
        /// Gets a read-only collection of project configurations.
        /// </summary>
        /// <value>A read-only collection of project configurations.</value>
        public IList<VSProjectConfiguration> Configurations => _configurations;

        /// <summary>
        /// Gets a read-only collection of all .cs files in the solution.
        /// </summary>
        /// <value>A read-only collection of all the .cs files in the solution.</value>
        public IList<VSProjectItem> Items => _items;

        /// <summary>
        /// Gets a read-only collection of project properties.
        /// </summary>
        /// <value>A read-only collection of project properties.</value>
        public IDictionary<string, string> Properties => _properties;

        public bool IsNetCoreProjectType { get; private set; }

        /// <summary>
        /// Loads the specified project file name.
        /// </summary>
        /// <param name="projectFileName">Name of the project file.</param>
        /// <returns>VSProject class containing project information.</returns>
        public static VSProjectDetails Load(string projectFileName)
        {
            projectFileName = projectFileName.Replace(@"\", "/");
            using (Stream stream = File.OpenRead(projectFileName))
            {
                VSProjectDetails data = new VSProjectDetails { _propertiesDictionary = true };

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true
                };

                using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                {
                    xmlReader.Read();
                    while (!xmlReader.EOF)
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.XmlDeclaration:
                                xmlReader.Read();
                                break;

                            case XmlNodeType.Element:
                                if (xmlReader.Name == "Project")
                                    data.ReadProject(projectFileName, xmlReader);

                                xmlReader.Read();
                                break;
                            default:
                                xmlReader.Read();
                                continue;
                        }
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// Finds the VisualStudio project configuration specified by a condition.
        /// </summary>
        /// <param name="condition">The condition which identifies the configuration
        ///  (example: " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ").</param>
        /// <returns><see cref="VSProjectConfiguration"/> object if found; <c>null</c> if no configuration was found that meets the
        /// specified condition.</returns>
        public VSProjectConfiguration FindConfiguration(string condition)
        {
            foreach (VSProjectConfiguration configuration in _configurations)
            {
                if (configuration?.Condition == null)
                    continue;
                if (configuration.Condition.IndexOf(condition, StringComparison.OrdinalIgnoreCase) >= 0)
                    return configuration;
            }

            return null;
        }

        private static VSProjectItem ReadItem(string projectName, XmlReader xmlReader, string itemType)
        {
            VSProjectItem item = new VSProjectItem(itemType) { Item = xmlReader["Include"] };

            if (xmlReader.IsEmptyElement == false)
            {
                xmlReader.Read();

                while (true)
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                        break;

                    ReadItemProperty(projectName, item, xmlReader);
                }
            }

            xmlReader.Read();

            return item;
        }

        private static void ReadItemProperty(string projectName, VSProjectItem item, XmlReader xmlReader)
        {
            string propertyName = xmlReader.Name;
            string propertyValue = xmlReader.ReadElementContentAsString();
            if (item.ItemProperties.ContainsKey(propertyName))
            {
                item.ItemProperties[propertyName] = propertyValue;
                Console.WriteLine($"Item {propertyName}:{propertyValue} already exists in project {projectName}");
            }
            else
            {
                item.ItemProperties.Add(propertyName, propertyValue);
            }
        }

        private void ReadProject(string projectName, XmlReader xmlReader)
        {
            var sdk = xmlReader.GetAttribute("Sdk");
            if (!string.IsNullOrEmpty(sdk))
            {
                IsNetCoreProjectType = true;
            }

            xmlReader.Read();

            while (xmlReader.NodeType != XmlNodeType.EndElement && xmlReader.EOF == false)
            {
                switch (xmlReader.Name)
                {
                    case "PropertyGroup":
                        if (_propertiesDictionary)
                        {
                            ReadPropertyGroup(xmlReader);
                            _propertiesDictionary = false;
                        }
                        else
                        {
                            _configurations.Add(ReadPropertyGroup(xmlReader));
                        }

                        xmlReader.Read();
                        break;
                    case "ItemGroup":
                        ReadItemGroup(projectName, xmlReader);
                        xmlReader.Read();
                        break;
                    default:
                        xmlReader.Read();
                        continue;
                }
            }
        }

        private VSProjectConfiguration ReadPropertyGroup(XmlReader xmlReader)
        {
            VSProjectConfiguration configuration = new VSProjectConfiguration();

            if (xmlReader["Condition"] != null && _propertiesDictionary == false)
            {
                configuration.Condition = xmlReader["Condition"];
            }

            xmlReader.Read();
            IDictionary<string, string> props = _propertiesDictionary ? _properties : configuration.Properties;
            while (xmlReader.NodeType != XmlNodeType.EndElement)
            {
                if (string.Equals(xmlReader.Name, "PublishDatabaseSettings", StringComparison.OrdinalIgnoreCase))
                {
                    xmlReader.Skip();
                    continue;
                }

                string name = xmlReader.Name;
                string val = xmlReader.ReadElementContentAsString();
                if (props.ContainsKey(name))
                {
                    continue;
                }

                props.Add(name, val);
            }

            return configuration;
        }

        private void ReadItemGroup(string projectName, XmlReader xmlReader)
        {
            xmlReader.Read();

            while (xmlReader.NodeType != XmlNodeType.EndElement && xmlReader.EOF == false)
            {
                switch (xmlReader.Name)
                {
                    case "Content":
                        VSProjectItem contentItem = ReadItem(projectName, xmlReader, VSProjectItem.Content);
                        _items.Add(contentItem);
                        break;

                    case "Compile":
                        VSProjectItem compileItems = ReadItem(projectName, xmlReader, VSProjectItem.CompileItem);
                        _items.Add(compileItems);
                        break;

                    case "None":
                        VSProjectItem noneItem = ReadItem(projectName, xmlReader, VSProjectItem.NoneItem);
                        _items.Add(noneItem);
                        break;

                    case "ProjectReference":
                        VSProjectItem projectReference = ReadItem(projectName, xmlReader, VSProjectItem.ProjectReference);
                        _items.Add(projectReference);
                        break;

                    case "Reference":
                        VSProjectItem reference = ReadItem(projectName, xmlReader, VSProjectItem.Reference);
                        _items.Add(reference);
                        break;

                    default:
                        xmlReader.Skip();
                        continue;
                }
            }
        }
    }
}