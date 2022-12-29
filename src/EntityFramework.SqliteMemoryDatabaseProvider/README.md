# SqliteMemoryDatabaseProvider

According to [Microsoft's documentation](https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#sqlite-in-memory), unit testing Entity Framework should be done with an in-memory database using SQLite. If you look at the link above, you'll see that there's a lot of boilerplate required to set up and use in-memory databases. This provider hopes to do much of the boilerplate behind the scenes so developers can write tests easier without having to worry about setting up their test databases correctly.

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