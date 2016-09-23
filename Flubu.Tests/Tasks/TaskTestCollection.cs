using Xunit;

namespace Flubu.Tests.Tasks
{
    [CollectionDefinition(nameof(TaskTestCollection))]
    public class TaskTestCollection : ICollectionFixture<TaskTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}