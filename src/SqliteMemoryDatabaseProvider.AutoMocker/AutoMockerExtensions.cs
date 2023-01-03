using Microsoft.EntityFrameworkCore;

namespace SqliteMemoryDatabaseProvider.AutoMocker;

/// <summary>
/// Extension methods for creating an in-memory database for an AutoMocker.
/// </summary>
public static class AutoMockerExtensions
{
    /// <summary>
    /// Creates an in-memory database to use while testing.
    /// </summary>
    /// <typeparam name="TInterface">Interface the database will implement.</typeparam>
    /// <typeparam name="TImplementation">Database implementation to create.</typeparam>
    /// <param name="mock">The mock that will use the in-memory database.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The created database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public static TImplementation CreateInMemoryDatabase<TInterface, TImplementation>(this Moq.AutoMock.AutoMocker mock, params object[] additionalParams)
        where TInterface : class
        where TImplementation : DbContext, TInterface
    {
        return mock.CreateInMemoryDatabase<TInterface, TImplementation>(null, additionalParams);
    }

    /// <summary>
    /// Creates an in-memory database to use while testing.
    /// </summary>
    /// <typeparam name="TInterface">Interface the database will implement.</typeparam>
    /// <typeparam name="TImplementation">Database implementation to create.</typeparam>
    /// <param name="mock">The mock that will use the in-memory database.</param>
    /// <param name="afterCreation">Optional. Actions to do after the database is created.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The created database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public static TImplementation CreateInMemoryDatabase<TInterface, TImplementation>(this Moq.AutoMock.AutoMocker mock, Action<TImplementation>? afterCreation = null, params object[] additionalParams)
        where TInterface : class
        where TImplementation : DbContext, TInterface
    {
        var provider = new SqliteMemoryDatabaseProvider(); 
        var database = provider.CreateDatabase(afterCreation, additionalParams);
        mock.Use<TInterface>(database);

        return database;
    }
    
    /// <summary>
    /// Creates an in-memory database to use while testing.
    /// </summary>
    /// <typeparam name="TInterface">Interface the database will implement.</typeparam>
    /// <typeparam name="TImplementation">Database implementation to create.</typeparam>
    /// <param name="mock">The mock that will use the in-memory database.</param>
    /// <param name="useSqliteConverters">Whether or not to use the type converters for SQLite.
    /// These converters include converting DateTimeOffsets, Decimals, and TimeSpans, as SQLite doesn't support these types.</param>
    /// <param name="afterCreation">Optional. Actions to do after the database is created.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The created database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public static TImplementation CreateInMemoryDatabase<TInterface, TImplementation>(this Moq.AutoMock.AutoMocker mock, bool useSqliteConverters = true, Action<TImplementation>? afterCreation = null, params object[] additionalParams)
        where TInterface : class
        where TImplementation : DbContext, TInterface
    {
        var provider = new SqliteMemoryDatabaseProvider(); 
        var database = provider.CreateDatabase(useSqliteConverters, afterCreation, additionalParams);
        mock.Use<TInterface>(database);

        return database;
    }
}