using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace FlubuCore.WebApi.Repository
{
    public class SerilogRepository : ISerilogRepository
    {
        private readonly LiteDatabase _db;

        public SerilogRepository(LiteDatabase db)
        {
            _db = db;
        }

        public List<string> GetExecuteScriptLogs(string traceIdentifier)
        {
            List<string> logs = new List<string>();

            var test = _db.GetCollection("log")
                .Find(Query.EQ("RequestId", traceIdentifier)).ToList();

            for (int i = 4; i < test.Count; i++)
            {
                logs.Add(test[i]["_m"]);
            }

        return logs;
    }
}
}
