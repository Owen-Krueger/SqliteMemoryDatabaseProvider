# SqliteMemoryDatabaseProvider

According to [Microsoft's documentation](https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#sqlite-in-memory), unit testing Entity Framework should be done with an in-memory database using SQLite. If you look at the link above, you'll see that there's a lot of boilerplate required to set up and use in-memory databases. This provider hopes to do much of the boilerplate behind the scenes so developers can write tests easier without having to worry about setting up their test databases correctly.

There are two packages offered within this repo:
- SqliteMemoryDatabaseProvider: Aids with creating in-memory databases for unit tests.
- SqliteMemoryDatabaseProvider.AutoMocker: Provides overloads for `AutoMocker` and `DbContext` to aid with creating in-memory databases for unit tests.

## Requirements

Using this package requires .NET 6.0 or newer.