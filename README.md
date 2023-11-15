# SqliteMemoryDatabaseProvider

According to [Microsoft's documentation](https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#sqlite-in-memory), unit testing Entity Framework should be done with an in-memory database using SQLite. If you look at the link above, you'll see that there's a lot of boilerplate required to set up and use in-memory databases. This provider hopes to do much of the boilerplate behind the scenes so developers can write tests easier without having to worry about setting up their test databases correctly.

There are two packages offered within this repo:
- SqliteMemoryDatabaseProvider: Aids with creating in-memory databases for unit tests.
- SqliteMemoryDatabaseProvider.AutoMocker: Provides overloads for `AutoMocker` and `DbContext` to aid with creating in-memory databases for unit tests.

## Requirements

Using these packages requires the project to be on .NET 6.0 or newer. These packages should only be used with projects using EntityFramework version 6.x.x or higher on .NET 6.0, version 7.x.x on .NET 7.0, or version 8.x.x on .NET 8.0.

## Contribute

If you encounter an issue or want to contribute to this package, please visit this package's [GitHub page](https://github.com/Owen-Krueger/SqliteMemoryDatabaseProvider).
