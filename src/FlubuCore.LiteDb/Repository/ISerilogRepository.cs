namespace FlubuCore.LiteDb.Repository
{
    using System.Collections.Generic;

    public interface ISerilogRepository
    {
        List<string> GetExecuteScriptLogs(string traceIdentifier);
    }
}
