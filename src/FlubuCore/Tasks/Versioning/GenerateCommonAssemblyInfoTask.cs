using System;
using System.IO;
using FlubuCore.Context;

namespace FlubuCore.Tasks.Versioning
{
    public class GenerateCommonAssemblyInfoTask : TaskBase<int, GenerateCommonAssemblyInfoTask>
    {
        private bool _generateAssemblyVersion;
        private bool _generateAssemblyVersionSet;
        private string _productRootDir;
        private string _companyName;
        private string _productName;
        private string _companyCopyright;
        private string _companyTrademark;
        private string _buildConfiguration;
        private bool _generateConfigurationAttribute = true;
        private bool _generateCultureAttribute;
        private int _productVersionFieldCount;
        private string _informationalVersion;
        private Version _buildVersion;
        private string _description;

        public GenerateCommonAssemblyInfoTask()
        {
        }

        public GenerateCommonAssemblyInfoTask(Version buildVersion)
        {
            _buildVersion = buildVersion;
        }

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Generates common assembly info file." : _description;
            set => _description = value;
        }

        public GenerateCommonAssemblyInfoTask BuildVersion(Version buildVersion)
        {
            _buildVersion = buildVersion;
            return this;
        }

        public GenerateCommonAssemblyInfoTask ProductRootDir(string productRoodDir)
        {
            _productRootDir = productRoodDir;
            return this;
        }

        public GenerateCommonAssemblyInfoTask CompanyName(string companyName)
        {
            _companyName = companyName;
            return this;
        }

        public GenerateCommonAssemblyInfoTask ProductName(string productName)
        {
            _productName = productName;
            return this;
        }

        public GenerateCommonAssemblyInfoTask CompanyCopyright(string companyCopyright)
        {
            _companyCopyright = companyCopyright;
            return this;
        }

        public GenerateCommonAssemblyInfoTask CompanyTrademark(string companyTrademark)
        {
            _companyTrademark = companyTrademark;
            return this;
        }

        public GenerateCommonAssemblyInfoTask BuildConfiguration(string buildConfiguration)
        {
            _buildConfiguration = buildConfiguration;
            return this;
        }

        public GenerateCommonAssemblyInfoTask GenerateConfigurationAttribute(bool generateConfigurationAttribute)
        {
            _generateConfigurationAttribute = generateConfigurationAttribute;
            return this;
        }

        public GenerateCommonAssemblyInfoTask GenerateCultureAttribute(bool generateCultureAttribute)
        {
            _generateCultureAttribute = generateCultureAttribute;
            return this;
        }

        public GenerateCommonAssemblyInfoTask GenerateAssemblyVersion(bool generateAssemblyVersion)
        {
            _generateAssemblyVersion = generateAssemblyVersion;
            _generateAssemblyVersionSet = true;
            return this;
        }

        public GenerateCommonAssemblyInfoTask ProductVersionFieldCount(int productVersionFieldCounet)
        {
            _productVersionFieldCount = productVersionFieldCounet;
            return this;
        }

        public GenerateCommonAssemblyInfoTask InformationalVersion(string informationVersion)
        {
            _informationalVersion = informationVersion;
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            if (string.IsNullOrEmpty(_buildConfiguration))
                _buildConfiguration = context.Properties.TryGet<string>(BuildProps.BuildConfiguration);

            if (_buildVersion == null)
                _buildVersion = context.Properties.GetBuildVersion().Version;

            if (string.IsNullOrEmpty(_companyCopyright))
                _companyCopyright = context.Properties.TryGet(DotNetBuildProps.CompanyCopyright, string.Empty);

            if (string.IsNullOrEmpty(_companyName))
                _companyName = context.Properties.TryGet(DotNetBuildProps.CompanyName, string.Empty);

            if (string.IsNullOrEmpty(_companyTrademark))
                _companyTrademark = context.Properties.TryGet(DotNetBuildProps.CompanyTrademark, string.Empty);

            if (string.IsNullOrEmpty(_productName))
            {
                string productId = context.Properties.TryGet<string>(BuildProps.ProductId);
                _productName = context.Properties.TryGet(DotNetBuildProps.ProductName, productId);
            }

            if (string.IsNullOrEmpty(_productRootDir))
                _productRootDir = context.Properties.TryGet(BuildProps.ProductRootDir, ".");

            if (!_generateAssemblyVersionSet)
                _generateAssemblyVersion = context.Properties.TryGet(DotNetBuildProps.AutoAssemblyVersion, true);

            if (string.IsNullOrEmpty(_informationalVersion))
                _informationalVersion = context.Properties.TryGet<string>(DotNetBuildProps.InformationalVersion);

            if (_productVersionFieldCount <= 0)
                _productVersionFieldCount = context.Properties.TryGet(DotNetBuildProps.ProductVersionFieldCount, 2);

            if (_buildVersion == null)
            {
                context.Fail("Assembly file version is not set.", 1);
                return 1;
            }

            using (Stream stream = File.Open(
                Path.Combine(_productRootDir, "CommonAssemblyInfo.cs"), FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(
                        $@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompanyAttribute(""{_companyName}"")]
[assembly: AssemblyProductAttribute(""{_productName}"")]
[assembly: AssemblyCopyrightAttribute(""{_companyCopyright}"")]
[assembly: AssemblyTrademarkAttribute(""{_companyTrademark}"")]
[assembly: AssemblyFileVersionAttribute(""{_buildVersion}"")]
[assembly: ComVisible(false)]");

                    string buildVersionShort = _buildVersion.ToString(_productVersionFieldCount);
                    string infVersion = _informationalVersion ?? buildVersionShort;

                    writer.WriteLine($"[assembly: AssemblyInformationalVersionAttribute(\"{infVersion}\")]");

                    if (_generateAssemblyVersion)
                        writer.WriteLine($"[assembly: AssemblyVersionAttribute(\"{buildVersionShort}\")]");

                    if (_generateConfigurationAttribute)
                        writer.WriteLine($"[assembly: AssemblyConfigurationAttribute(\"{_buildConfiguration}\")]");

                    if (_generateCultureAttribute)
                        writer.WriteLine("[assembly: AssemblyCultureAttribute(\"\")]");
                }
            }

            return 0;
        }
    }
}