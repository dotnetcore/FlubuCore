namespace FlubuCore.LiteDb.Repository
{
    using LiteDB;

    public interface ILiteRepositoryFactory
    {
        LiteDatabase CreateLiteDatabase(string connectionString);
    }
}
