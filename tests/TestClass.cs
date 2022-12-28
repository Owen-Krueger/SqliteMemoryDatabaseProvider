namespace EntityFramework.SqliteMemoryDatabaseProvider.UnitTests;

internal class TestClass
{
    private readonly ITestEntities testEntities;

    public TestClass(ITestEntities testEntities)
    {
        this.testEntities = testEntities;
    }

    internal bool RecordExists(string property)
    {
        return testEntities.TestModels.Any(x => x.OtherProperty == property);
    }
}