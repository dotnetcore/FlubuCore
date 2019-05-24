namespace FlubuCore.Packaging
{
    public interface IPackageProcessor : IFilterable
    {
        IPackageDef Process(IPackageDef packageDef);
    }
}