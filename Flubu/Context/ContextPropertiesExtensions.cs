using System;
using System.Collections.Generic;

namespace Flubu.Context
{
    public static class ContextPropertiesExtensions
    {
        public const string BuildVersion = "build_version";
        public const string BuildConfiguration = "build_configuration";
        public const string CompanyCopyright = "company_copyright";
        public const string CompanyName = "company_name";
        public const string CompanyTrademark = "company_trademark";
        public const string ProductId = "product_id";
        public const string ProductName = "product_name";
        public const string ProductRootDir = "product_root_dir";
        public const string AutoAssemblyVersion = "auto_assembly_version";
        public const string InformationalVersion = "informational_version";
        public const string ProductVersionFieldCount = "product_version_field_count";

        public static Version GetBuildVersion(this ITaskContext context)
        {
            return context.Properties.TryGet<Version>(BuildVersion);
        }

        public static Version SetBuildVersion(this ITaskContext context, Version version)
        {
            context.Properties.Set<Version>(BuildVersion, version);
            return version;
        }

        public static T TryGet<T>(this ITaskContext context, string propName, T defaultValue = default(T))
        {
            T ret = context.Properties.TryGet<T>(propName);

            if (EqualityComparer<T>.Default.Equals(ret, default(T)))
                ret = defaultValue;

            return ret;
        }

        public static T Set<T>(this ITaskContext context, string propName, T value)
        {
            context.Properties.Set<T>(propName, value);
            return value;
        }
    }
}
