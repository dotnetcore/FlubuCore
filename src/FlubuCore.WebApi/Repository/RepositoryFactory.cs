using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Infrastructure;

namespace FlubuCore.WebApi.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ILiteRepositoryFactory _liteRepositoryFactory;

        private readonly ITimeProvider _timeProvider;

        public RepositoryFactory(ILiteRepositoryFactory liteRepositoryFactory, ITimeProvider timeProvider)
        {
            _liteRepositoryFactory = liteRepositoryFactory;
            _timeProvider = timeProvider;
        }

        public ISerilogRepository CreateSerilogRepository()
        {
            var connStr = $"FileName=Logs/log_{_timeProvider.Now.Date:yyyy-dd-M}.db;Upgrade=true;Connection=Shared";
            return new SerilogRepository(_liteRepositoryFactory.CreateLiteDatabase(connStr));
        }
    }
}
