namespace FlubuCore.LiteDb.Repository
{
    public interface IRepositoryFactory
    {
        ISerilogRepository CreateSerilogRepository();
    }
}
