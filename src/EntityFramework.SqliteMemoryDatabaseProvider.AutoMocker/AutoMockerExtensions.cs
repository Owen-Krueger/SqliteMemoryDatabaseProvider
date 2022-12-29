using Microsoft.EntityFrameworkCore;

namespace EntityFramework.SqliteMemoryDatabaseProvider.AutoMocker;

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
    public static TImplementation CreateInMemoryDatabase<TInterface, TImplementation>(this Moq.AutoMock.AutoMocker mock, Action<TImplementation>? afterCreation = null, params object[] additionalParams)
        where TInterface : class
        where TImplementation : DbContext, TInterface
    {
        var provider = new SqliteMemoryDatabaseProvider(); 
        var database = provider.CreateDatabase(afterCreation, additionalParams);
        mock.Use<TInterface>(database);

        return database;
    }
}