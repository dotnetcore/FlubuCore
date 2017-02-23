using System;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Context.FluentInterface.TaskExtensions
{
    public partial class TaskExtensionsFluentInterface
    {
        public ITaskExtensionsFluentInterface GenerateCommonAssemblyInfo()
        {
            string buildConfiguration = Context.Properties.Get<string>(BuildProps.BuildConfiguration);
            string companyCopyright = Context.Properties.Get(BuildProps.CompanyCopyright, string.Empty);
            string companyName = Context.Properties.Get(BuildProps.CompanyName, string.Empty);
            string companyTrademark = Context.Properties.Get(BuildProps.CompanyTrademark, string.Empty);
            string productId = Context.Properties.Get<string>(BuildProps.ProductId);
            string productName = Context.Properties.Get(BuildProps.ProductName, productId);
            bool generateAssemblyVersion = Context.Properties.Get(BuildProps.AutoAssemblyVersion, true);

            GenerateCommonAssemblyInfoTask task = Context.Tasks().GenerateCommonAssemblyInfoTask()
                .BuildConfiguration(buildConfiguration)
                .CompanyCopyright(companyCopyright)
                .CompanyName(companyName)
                .CompanyTrademark(companyTrademark)
                .GenerateConfigurationAttribute(true)
                .ProductName(productName);

            if (Context.Properties.Has(BuildProps.InformationalVersion))
                task.InformationalVersion(Context.Properties.Get<string>(BuildProps.InformationalVersion));

            task.ProductVersionFieldCount(Context.Properties.Get(BuildProps.ProductVersionFieldCount, 2))
                .GenerateAssemblyVersion(generateAssemblyVersion)
                .Execute(Context);

            return this;
        }
    }
}
