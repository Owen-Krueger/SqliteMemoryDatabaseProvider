using Moq.AutoMock;
using NUnit.Framework;

namespace EntityFramework.SqliteMemoryDatabaseProvider.UnitTests;

internal class MemoryDatabaseProviderTests
{
    private readonly TestModel testRecord = new() { OtherProperty = "Test" };
    private SqliteMemoryDatabaseProvider sqliteMemoryDatabaseProvider;

    [SetUp]
    public void SetUp()
    {
        sqliteMemoryDatabaseProvider = new SqliteMemoryDatabaseProvider();
    }

    [TearDown]
    public void TearDown()
    {
        sqliteMemoryDatabaseProvider.Dispose();
    }

    [Test]
    public async Task CreateDatabase_NoArguments_DatabaseReturned()
    {
        var mock = new AutoMocker();
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>();
        database.TestModels.Add(testRecord);
        await database.SaveChangesAsync();
        mock.Use<ITestEntities>(database);
        var testClass = mock.CreateInstance<TestClass>();
        var result = testClass.RecordExists(testRecord.OtherProperty);
        Assert.IsTrue(result);
    }

    [Test]
    public void CreateDatabase_RowsAddedInAction_RowsPresentAfterCreation()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>(x =>
        {
            x.TestModels.Add(testRecord);
        });
        var record = database.TestModels.First();
        Assert.AreEqual(testRecord.OtherProperty, record.OtherProperty);
    }

    [TestCase("A")]
    [TestCase("B")]
    [TestCase("C")]
    public async Task CreateDatabase_MultipleCases_DatabaseIndependentPerRun(string property)
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>();
        database.TestModels.Add(new TestModel() { OtherProperty = property });
        await database.SaveChangesAsync();
        Assert.AreEqual(1, database.TestModels.Count());
        Assert.AreEqual(property, database.TestModels.First().OtherProperty);
    }

    [Test]
    public void CreateDatabaseUsingClassProvider_DatabaseA_IndependentFromDatabaseB()
    {
        var database = sqliteMemoryDatabaseProvider.CreateDatabase<TestEntities>(x =>
            x.TestModels.Add(testRecord)
        );
        Assert.AreEqual(1, database.TestModels.Count());
        var record = database.TestModels.First();
        Assert.AreEqual(testRecord.OtherProperty, record.OtherProperty);
    }

    [Test]
    public void CreateDatabaseUsingClassProvider_DatabaseB_IndependentFromDatabaseA()
    {
        var testModel = new TestModel() { OtherProperty = "DifferentFromA" };
        var database = sqliteMemoryDatabaseProvider.CreateDatabase<TestEntities>(x =>
            x.TestModels.Add(testModel)
        );
        Assert.AreEqual(1, database.TestModels.Count());
        var record = database.TestModels.First();
        Assert.AreEqual(testModel.OtherProperty, record.OtherProperty);
    }

    [Test]
    public async Task CreateDatabase_DatabaseWithAdditionalParameters_ParametersPassedToConstructor()
    {
        var mock = new AutoMocker();
        var testModelWithDate = new TestModelWithDate() { Date = DateTimeOffset.Now };
        using var provider = new SqliteMemoryDatabaseProvider();
        var dateTimeConverterMock = mock.GetMock<IDateTimeConverter>();
        var database = provider.CreateDatabase<ComplexTestEntities>(dateTimeConverterMock.Object);
        database.TestModels.Add(testModelWithDate);
        await database.SaveChangesAsync();
        var record = database.TestModels.First();
        Assert.AreEqual(testModelWithDate.Date, record.Date);
    }

    [Test]
    public void CreateDatabase_DatabaseWithAdditionalParametersAndAction_ParametersPassedToConstructor()
    {
        var mock = new AutoMocker();
        var testModelWithDate = new TestModelWithDate() { Date = DateTimeOffset.Now };
        using var provider = new SqliteMemoryDatabaseProvider();
        var dateTimeConverterMock = mock.GetMock<IDateTimeConverter>();
        var database = provider.CreateDatabase<ComplexTestEntities>(x => x.TestModels.Add(testModelWithDate), dateTimeConverterMock.Object);
        var record = database.TestModels.First();
        Assert.AreEqual(testModelWithDate.Date, record.Date);
    }
}