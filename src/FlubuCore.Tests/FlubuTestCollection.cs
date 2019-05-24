using Xunit;

namespace FlubuCore.Tests
{
    [CollectionDefinition(nameof(FlubuTestCollection))]
    public class FlubuTestCollection : ICollectionFixture<FlubuTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}