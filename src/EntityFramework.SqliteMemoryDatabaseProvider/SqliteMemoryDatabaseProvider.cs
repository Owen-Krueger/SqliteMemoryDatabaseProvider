using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
    /// <typeparam name="T">The type of database to create.</typeparam>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public T CreateDatabase<T>(params object[] additionalParams)
        where T : DbContext
    {
        return CreateDatabase<T>(null, additionalParams);
    }

    /// <summary>
    /// Creates a new in-memory database.
    /// </summary>
    /// <typeparam name="T">The type of database to create.</typeparam>
    /// <param name="afterCreation">Optional. Actions to do after the database is created.</param>
    /// <param name="additionalParams">Additional parameters to use when creating the database instance.</param>
    /// <returns>The newly created in-memory database.</returns>
    /// <exception cref="DatabaseCreationException">Cannot create new database. See inner exception for details.</exception>
    public T CreateDatabase<T>(Action<T>? afterCreation = null, params object[] additionalParams)
        where T : DbContext
    {
        try
        {
            var options = new DbContextOptionsBuilder<T>().UseSqlite(connection).Options;
            var parameters = new object[] { options };
            parameters = parameters.Concat(additionalParams).ToArray();
            if (Activator.CreateInstance(typeof(T), args: parameters) is not T database)
            {
                throw new DatabaseCreationException("Failed to create in-memory database",
                    new InvalidCastException($"Could not cast database to type {typeof(T)}"));
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