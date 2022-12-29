using EntityFramework.SqliteMemoryDatabaseProvider.AutoMocker;
using NUnit.Framework;

namespace EntityFramework.SqliteMemoryDatabaseProvider.UnitTests;

public class AutoMockerExtensionTests
{
    private readonly TestModel testRecord = new() { OtherProperty = "Test" };

        [Test]
        public void CreateInMemoryDatabase_NoActionProvided_NoRowsPresentAfterCreation()
        {
            var mock = new Moq.AutoMock.AutoMocker();
            var database = mock.CreateInMemoryDatabase<ITestEntities, TestEntities>();
            var testClass = mock.CreateInstance<TestClass>();
            var result = testClass.RecordExists(testRecord.OtherProperty);
            Assert.That(result, Is.False);
            var record = database.TestModels.FirstOrDefault();
            Assert.That(record, Is.Null);
            database.CloseInMemoryDatabaseConnection();
        }

        [Test]
        public void CreateInMemoryDatabase_RowsAddedAfterCreation_RowsPresent()
        {
            var mock = new Moq.AutoMock.AutoMocker();
            var database = mock.CreateInMemoryDatabase<ITestEntities, TestEntities>();
            database.TestModels.Add(testRecord);
            database.SaveChanges();
            var testClass = mock.CreateInstance<TestClass>();
            var result = testClass.RecordExists(testRecord.OtherProperty);
            Assert.That(result, Is.True);
            var record = database.TestModels.First();
            Assert.That(record.OtherProperty, Is.EqualTo(testRecord.OtherProperty));
            database.CloseInMemoryDatabaseConnection();
        }

        [Test]
        public void CreateInMemoryDatabase_RowsAddedInAction_RowsPresentAfterCreation()
        {
            var mock = new Moq.AutoMock.AutoMocker();
            var database = mock.CreateInMemoryDatabase<ITestEntities, TestEntities>(x =>
                x.TestModels.Add(testRecord)
            );
            var testClass = mock.CreateInstance<TestClass>();
            var result = testClass.RecordExists(testRecord.OtherProperty);
            Assert.That(result, Is.True);
            var record = database.TestModels.First();
            Assert.That(record.OtherProperty, Is.EqualTo(testRecord.OtherProperty));
            DbContextExtensions.CloseInMemoryDatabaseConnections(database);
        }

        [Test]
        public async Task CreateInMemoryDatabase_DatabaseWithAdditionalParameters_ParametersPassedToConstructor()
        {
            var mock = new Moq.AutoMock.AutoMocker();
            var testModelWithDate = new TestModelWithDate() { Date = DateTimeOffset.Now };
            var dateTimeConverterMock = mock.GetMock<IDateTimeConverter>();
            var database = mock.CreateInMemoryDatabase<IComplexTestEntities, ComplexTestEntities>(dateTimeConverterMock.Object);
            database.TestModels.Add(testModelWithDate);
            await database.SaveChangesAsync();
            var record = database.TestModels.First();
            Assert.That(record.Date, Is.EqualTo(testModelWithDate.Date));
            database.CloseInMemoryDatabaseConnection();
        }

        [Test]
        public void CreateInMemoryDatabase_DatabaseWithAdditionalParametersAndAction_ParametersPassedToConstructor()
        {
            var mock = new Moq.AutoMock.AutoMocker();
            var testModelWithDate = new TestModelWithDate() { Date = DateTimeOffset.Now };
            var dateTimeConverterMock = mock.GetMock<IDateTimeConverter>();
            var database = mock.CreateInMemoryDatabase<IComplexTestEntities, ComplexTestEntities>(x => x.TestModels.Add(testModelWithDate), dateTimeConverterMock.Object);
            var record = database.TestModels.First();
            Assert.That(record.Date, Is.EqualTo(testModelWithDate.Date));
        }
}