# SqliteMemoryDatabaseProvider

This provider to aid with creating in-memory databases for unit tests.

## Create Database

You can set up a new `SqliteMemoryDatabaseProvider` and call `CreateDatabase`. This will set up the database in-memory to be used in your test. `SqliteMemoryDatabaseProvider` needs to be instantiated so it can open up a new database connection using in-memory SQLite.

An example of this in a test:

``` C#
var mock = new AutoMocker();
using var provider = new SqliteMemoryDatabaseProvider();
var mockDatabase = provider.CreateDatabase<TestEntities>();
mockDatabase.TestModels.Add(testRecord);
await mockDatabase.SaveChangesAsync();
mock.Use<ITestEntities>(mockDatabase);
var testClass = mock.CreateInstance<TestClass>();
```

Optionally, you can provide actions to execute after the database is created. Commonly, this can be used to populate data into your new database. These changes are saved automatically.

An example of this in a test:

``` C#
var mock = new AutoMocker();
using var provider = new SqliteMemoryDatabaseProvider();
var mockDatabase = provider.CreateDatabase<TestEntities>(x =>
    x.TestModels.Add(testRecord);
);
mock.Use<ITestEntities>(mockDatabase);
var testClass = mock.CreateInstance<TestClass>();
```

`SqliteMemoryDatabaseProvider` can also be used at a class level for your tests:

``` C#
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
```

This can help simplify your tests like so:

``` C# 
var mock = new AutoMocker();
var mockDatabase = sqliteMemoryDatabaseProvider.CreateDatabase<TestEntities>(x =>
    x.TestModels.Add(testRecord);
);
mock.Use<ITestEntities>(mockDatabase);
var testClass = mock.CreateInstance<TestClass>();
```

### Additional Parameters

If your entity needs additional parameters, an overload is available to provide them:

``` C#
var mock = new AutoMocker();
var mockedDependency = mock.GetMock<ITestDependencies>();
using var provider = new SqliteMemoryDatabaseProvider();
var mockDatabase = provider.CreateDatabase<TestEntities>(mockedDependency.Object);
await mockDatabase.SaveChangesAsync();
var testClass = mock.CreateInstance<TestClass>();
```

You can optionally combine providing actions and additional constructor parameters.

``` C#
var mock = new AutoMocker();
var mockedDependency = mock.GetMock<ITestDependencies>();
using var provider = new SqliteMemoryDatabaseProvider();
var mockDatabase = provider.CreateDatabase<TestEntities>(x => {
    x.TestModels.Add(testRecord);
}, mockedDependency.Object);
await mockDatabase.SaveChangesAsync();
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

`SqliteMemoryDatabaseProvider` will automatically close its database connections when it's being disposed. The simplest ways to do this are to either set up your provider at the class level or scope the provider in your test to dispose when done by using the `using` keyword:

### Scope Provider

``` C#
using var provider = new SqliteMemoryDatabaseProvider();
```

When the test completes, the database connection will automatically be closed and disposed.

### Class Level

You can also set up the `SqliteMemoryDatabaseProvider` at the class level and call its `Dispose` method during test teardown:

``` C#
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
```

## Contribute

If you encounter an issue or want to contribute to this package, please visit this package's [GitHub page](https://github.com/Owen-Krueger/SqliteMemoryDatabaseProvider).