# SqliteMemoryDatabaseProvider.AutoMocker

This package provides overloads for `AutoMocker` and `DbContext` to aid with creating in-memory databases for unit tests.

## Create Database

`CreateInMemoryDatabase` is an overload for `AutoMocker` that allows you to create a database in-memory using SQLite and tell the mock on the test to use it.

Optionally, you can provide actions to execute after the database is created. Commonly, this can be used to populate data into your new database. These changes are saved automatically.

An example of this in a test:

``` C#
var mock = new AutoMocker();
mock.CreateInMemoryDatabase<ITestEntities, TestEntities>(x =>
    x.TestModels.Add(testRecord)
);
var testClass = mock.CreateInstance<TestClass>();
```

In the above example, `ITestEntities` is injected into `TestClass`. When `CreateInMemoryDatabase` is called, a new `TestEntities` is created in memory, the `testRecord` is inserted, and then mock is told to use the new `TestEntities` as an implementation for `ITestEntities`.

This method also returns the database if you want to modify or get data from it after it's created.

``` C#
var database = mock.CreateInMemoryDatabase<ITestEntities, TestEntities>();
database.TestModels.Add(testRecord);
database.SaveChanges();
var testClass = mock.CreateInstance<TestClass>();
var records = database.TestModels.ToList();
database.CloseInMemoryDatabaseConnection();
```

### Additional Parameters

If your entity needs additional parameters, an overload is available to provide them:

``` C#
var mock = new AutoMocker();
var mockedDependency = mock.GetMock<ITestDependencies>();
mock.CreateInMemoryDatabase<ITestEntities, TestEntities>(mockedDependency.Object);
var testClass = mock.CreateInstance<TestClass>();
```

You can optionally combine providing actions and additional constructor parameters.

``` C#
var mock = new AutoMocker();
var mockedDependency = mock.GetMock<ITestDependencies>();
mock.CreateInMemoryDatabase<ITestEntities, TestEntities>(x => {
    x.TestModels.Add(testRecord)
}, mockedDependency.Object);
var testClass = mock.CreateInstance<TestClass>();
```

### SQLite Type Converters
By default, [SQLite doesn't support certain types, such as DateTimeOffset, Decimal, and TimeSpan](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations). This package automatically sets up converters by these when setting up the SQLite database. You can choose to not utilize these converters by using an overload:

``` C#
using var provider = new SqliteMemoryDatabaseProvider();
var mockDatabase = provider.CreateDatabase<TestEntities>(false, x =>
    x.TestModels.Add(testRecord);
);
```

## Closing In-Memory Database Connections

Although tests are short lived, it's still best practice to close database connections upon test completion.

Two extension methods are provided to close in-memory database connections for your `DbContext`.

`CloseInMemoryDatabaseConnection` will close a singular connection from the generated database.

``` C#
var database = mock.CreateInMemoryDatabase<ITestEntities, TestEntities>();
database.CloseInMemoryDatabaseConnection();
```

`CloseInMemoryDatabaseConnections` will close one or many database connections from the inputted `DbContext` instances. This can be useful if multiple databases were set up in your tests.

``` C#
var database1 = mock.CreateInMemoryDatabase<ITestEntities1, TestEntities1>();
var database2 = mock.CreateInMemoryDatabase<ITestEntities2, TestEntities2>();
DbContextExtensions.CloseInMemoryDatabaseConnections(database1, database2);
```