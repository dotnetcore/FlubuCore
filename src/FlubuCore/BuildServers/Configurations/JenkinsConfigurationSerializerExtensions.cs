using System;
using System.Text;

namespace FlubuCore.BuildServers.Configurations
{
    internal static class JenkinsConfigurationSerializerExtensions
    {
        internal static StringBuilder AppendBlockWithNewLine(this StringBuilder builder, string blockName, Action<StringBuilder> block, int indentSize = 0)
        {
            AppendBlock(builder, blockName, block, indentSize);
            builder.AppendLine();
            return builder;
        }

        internal static StringBuilder AppendBlock(this StringBuilder builder, string blockName, Action<StringBuilder> block, int indentSize = 0)
        {
            string indent = null;
            if (indentSize != 0)
            {
                indent = new string(' ', indentSize);
                builder.Append(indent);
            }

            builder.Append(blockName);
            builder.AppendLine(" {");
            block.Invoke(builder);
            if (indentSize != 0)
            {
                builder.Append(indent);
            }

            builder.AppendLine("}");
            return builder;
        }

        internal static StringBuilder AppendIndent(this StringBuilder sb, int indent)
        {
            sb.Append(new string(' ', indent));
            return sb;
        }
    }
}
