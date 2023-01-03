using AutoFixture;
using AutoFixture.AutoMoq;
using NUnit.Framework;

namespace EntityFramework.SqliteMemoryDatabaseProvider.UnitTests;

internal class MemoryDatabaseProviderTests
{
    private readonly TestModel testRecord = new() { OtherProperty = "Test" };
    private SqliteMemoryDatabaseProvider? sqliteMemoryDatabaseProvider;
    private IFixture fixture;

    [SetUp]
    public void SetUp()
    {
        sqliteMemoryDatabaseProvider = new SqliteMemoryDatabaseProvider();
        fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [TearDown]
    public void TearDown()
    {
        sqliteMemoryDatabaseProvider?.Dispose();
    }

    [Test]
    public async Task CreateDatabase_NoArguments_DatabaseReturned()
    {
        var mock = new Moq.AutoMock.AutoMocker();
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>();
        database.TestModels.Add(testRecord);
        await database.SaveChangesAsync();
        mock.Use<ITestEntities>(database);
        var testClass = mock.CreateInstance<TestClass>();
        var result = testClass.RecordExists(testRecord.OtherProperty);
        Assert.That(result, Is.True);
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
        Assert.That(record.OtherProperty, Is.EqualTo(testRecord.OtherProperty));
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
        Assert.That(database.TestModels.Count(), Is.EqualTo(1));
        Assert.That(database.TestModels.First().OtherProperty, Is.EqualTo(property));
    }

    [Test]
    public void CreateDatabaseUsingClassProvider_DatabaseA_IndependentFromDatabaseB()
    {
        var database = sqliteMemoryDatabaseProvider?.CreateDatabase<TestEntities>(x =>
            x.TestModels.Add(testRecord)
        );
        Assert.That(database?.TestModels.Count(), Is.EqualTo(1));
        var record = database?.TestModels.First();
        Assert.That(record?.OtherProperty, Is.EqualTo(testRecord.OtherProperty));
    }

    [Test]
    public void CreateDatabaseUsingClassProvider_DatabaseB_IndependentFromDatabaseA()
    {
        var testModel = new TestModel() { OtherProperty = "DifferentFromA" };
        var database = sqliteMemoryDatabaseProvider?.CreateDatabase<TestEntities>(x =>
            x.TestModels.Add(testModel)
        );
        Assert.That(database?.TestModels.Count(), Is.EqualTo(1));
        var record = database?.TestModels.First();
        Assert.That(record?.OtherProperty, Is.EqualTo(testModel.OtherProperty));
    }

    [Test]
    public async Task CreateDatabase_DatabaseWithAdditionalParameters_ParametersPassedToConstructor()
    {
        var mock = new Moq.AutoMock.AutoMocker();
        var testModelWithDate = new TestModelWithDate() { Date = DateTimeOffset.Now };
        using var provider = new SqliteMemoryDatabaseProvider();
        var dateTimeConverterMock = mock.GetMock<IDateTimeConverter>();
        var database = provider.CreateDatabase<ComplexTestEntities>(dateTimeConverterMock.Object);
        database.TestModels.Add(testModelWithDate);
        await database.SaveChangesAsync();
        var record = database.TestModels.First();
        Assert.That(record.Date, Is.EqualTo(testModelWithDate.Date));
    }

    [Test]
    public void CreateDatabase_DatabaseWithAdditionalParametersAndAction_ParametersPassedToConstructor()
    {
        var mock = new Moq.AutoMock.AutoMocker();
        var testModelWithDate = new TestModelWithDate() { Date = DateTimeOffset.Now };
        using var provider = new SqliteMemoryDatabaseProvider();
        var dateTimeConverterMock = mock.GetMock<IDateTimeConverter>();
        var database = provider.CreateDatabase<ComplexTestEntities>(x => x.TestModels.Add(testModelWithDate), dateTimeConverterMock.Object);
        var record = database.TestModels.First();
        Assert.That(record.Date, Is.EqualTo(testModelWithDate.Date));
    }

    [Test]
    public void CreateDatabase_ParametersMissing_DatabaseCreationExceptionThrown()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        Assert.Throws<DatabaseCreationException>(() => provider.CreateDatabase<ComplexTestEntities>());
    }
    
    [Test]
    public async Task CreateDatabase_DateTimeOffsetWithConverter_DateConverted()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>();
        List<TestModel> models = new()
        {
            fixture.Build<TestModel>().With(x => x.Date, DateTimeOffset.Now.AddDays(-1)).Create(),
            fixture.Build<TestModel>().With(x => x.Date, DateTimeOffset.Now).Create(),
        };
        database.TestModels.AddRange(models);
        await database.SaveChangesAsync();
        var records = database.TestModels.OrderByDescending(x => x.Date).ToList();
        Assert.That(records, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(records[0], Is.EqualTo(models[1]));
            Assert.That(records[1], Is.EqualTo(models[0]));
        });
    }
    
    [Test]
    public async Task CreateDatabase_DateTimeOffsetWithoutConverter_NotSupportedExceptionThrown()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>(false);
        var models = fixture.CreateMany<TestModel>();
        database.TestModels.AddRange(models);
        await database.SaveChangesAsync();
        Assert.Throws<NotSupportedException>(() => database.TestModels.OrderByDescending(x => x.Date).ToList());
    }
    
    [Test]
    public async Task CreateDatabase_DecimalWithConverter_DecimalConverted()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>();
        List<TestModel> models = new()
        {
            fixture.Build<TestModel>().With(x => x.Decimal, -6541m).Create(),
            fixture.Build<TestModel>().With(x => x.Decimal, 9846).Create(),
        };
        database.TestModels.AddRange(models);
        await database.SaveChangesAsync();
        var records = database.TestModels.OrderByDescending(x => x.Decimal).ToList();
        Assert.That(records, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(records[0], Is.EqualTo(models[1]));
            Assert.That(records[1], Is.EqualTo(models[0]));
        });
    }
    
    [Test]
    public async Task CreateDatabase_DecimalWithoutConverter_NotSupportedExceptionThrown()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>(false);
        var models = fixture.CreateMany<TestModel>();
        database.TestModels.AddRange(models);
        await database.SaveChangesAsync();
        Assert.Throws<NotSupportedException>(() => database.TestModels.OrderByDescending(x => x.Decimal).ToList());
    }
    
    [Test]
    public async Task CreateDatabase_TimespanWithConverter_DateConverted()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>();
        List<TestModel> models = new()
        {
            fixture.Build<TestModel>().With(x => x.TimeSpan, TimeSpan.FromDays(-1)).Create(),
            fixture.Build<TestModel>().With(x => x.TimeSpan, new TimeSpan()).Create(),
        };
        database.TestModels.AddRange(models);
        await database.SaveChangesAsync();
        var records = database.TestModels.OrderByDescending(x => x.TimeSpan).ToList();
        Assert.That(records, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(records[0], Is.EqualTo(models[1]));
            Assert.That(records[1], Is.EqualTo(models[0]));
        });
    }
    
    [Test]
    public async Task CreateDatabase_TimespanWithoutConverter_NotSupportedExceptionThrown()
    {
        using var provider = new SqliteMemoryDatabaseProvider();
        var database = provider.CreateDatabase<TestEntities>(false);
        var models = fixture.CreateMany<TestModel>();
        database.TestModels.AddRange(models);
        await database.SaveChangesAsync();
        Assert.Throws<NotSupportedException>(() => database.TestModels.OrderByDescending(x => x.TimeSpan).ToList());
    }
}