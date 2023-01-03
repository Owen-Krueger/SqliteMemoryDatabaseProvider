using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFramework.SqliteMemoryDatabaseProvider;

/// <summary>
/// Provider that will spin up in-memory databases for testing.
/// </summary>
public sealed class SqliteMemoryDatabaseProvider : IDisposable
{
    private readonly SqliteConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteMemoryDatabaseProvider"/> class.
    /// </summary>
    public SqliteMemoryDatabaseProvider()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
    }

    /// <summary>
    /// Creates a new in-memory database.
    /// </summary>
    /// <typeparam name="TContext">The type of database to create.</typeparam>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public TContext CreateDatabase<TContext>(params object[] additionalParams)
        where TContext : DbContext
    {
        return CreateDatabaseInner<TContext>(true , null, additionalParams);
    }

    /// <summary>
    /// Creates a new in-memory database.
    /// </summary>
    /// <typeparam name="TContext">The type of database to create.</typeparam>
    /// <param name="useSqliteConverters">Whether or not to use the type converters for SQLite.
    /// These converters include converting DateTimeOffsets, Decimals, and TimeSpans, as SQLite doesn't support these types.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public TContext CreateDatabase<TContext>(bool useSqliteConverters, params object[] additionalParams)
        where TContext : DbContext
    {
        return CreateDatabaseInner<TContext>(useSqliteConverters, null, additionalParams);
    }
    
    /// <summary>
    /// Creates a new in-memory database.
    /// </summary>
    /// <typeparam name="TContext">The type of database to create.</typeparam>
    /// <param name="afterCreation">Optional. Actions to do after the database is created.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public TContext CreateDatabase<TContext>(Action<TContext>? afterCreation = null, params object[] additionalParams)
        where TContext : DbContext
    {
        return CreateDatabaseInner(true, afterCreation, additionalParams);
    }

    /// <summary>
    /// Creates a new in-memory database.
    /// </summary>
    /// <typeparam name="TContext">The type of database to create.</typeparam>
    /// <param name="useSqliteConverters">Whether or not to use the type converters for SQLite.
    /// These converters include converting DateTimeOffsets, Decimals, and TimeSpans, as SQLite doesn't support these types.</param>
    /// <param name="afterCreation">Optional. Actions to do after the database is created.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public TContext CreateDatabase<TContext>(bool useSqliteConverters = true, Action<TContext>? afterCreation = null, params object[] additionalParams)
        where TContext : DbContext
    {
        return CreateDatabaseInner(useSqliteConverters, afterCreation, additionalParams);
    }

    /// <summary>
    /// Creates a new in-memory database.
    /// </summary>
    /// <typeparam name="TContext">The type of database to create.</typeparam>
    /// <param name="useSqliteConverters">Whether or not to use the type converters for SQLite.
    /// These converters include converting DateTimeOffsets, Decimals, and TimeSpans, as SQLite doesn't support these types.</param>
    /// <param name="afterCreation">Optional. Actions to do after the database is created.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    private TContext CreateDatabaseInner<TContext>(bool useSqliteConverters = true, Action<TContext>? afterCreation = null, params object[] additionalParams)
        where TContext : DbContext
    {
        try
        {
            var builder = new DbContextOptionsBuilder<TContext>().UseSqlite(connection);
            if (useSqliteConverters)
            {
                builder.ReplaceService<IModelCustomizer, SqliteContextCustomizer>();
            }
            var parameters = new object[] { builder.Options };
            parameters = parameters.Concat(additionalParams).ToArray();
            if (Activator.CreateInstance(typeof(TContext), args: parameters) is not TContext database)
            {
                throw new DatabaseCreationException("Failed to create in-memory database",
                    new InvalidCastException($"Could not cast database to type {typeof(TContext)}"));
            }
            
            database.Database.EnsureCreated();

            if (afterCreation == null)
            {
                return database;
            }
            
            afterCreation(database);
            database.SaveChanges();
            return database;
        }
        catch (Exception e)
        {
            throw new DatabaseCreationException("Failed to create in-memory database", e);
        }
    }
    
    /// <summary>
    /// Disposes of the in-memory connection used by the databases.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal implementation to dispose of the connection.
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            connection.Dispose();
        }
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~SqliteMemoryDatabaseProvider()
    {
        Dispose(disposing: false);
    }
}