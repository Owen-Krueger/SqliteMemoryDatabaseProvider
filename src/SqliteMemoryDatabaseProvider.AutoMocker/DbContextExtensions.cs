using Microsoft.EntityFrameworkCore;

namespace SqliteMemoryDatabaseProvider.AutoMocker;

/// <summary>
/// Extensions for a DbContext to handle in-memory databases.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Closes the database connection used by the in-memory database.
    /// </summary>
    /// <param name="database">The database which connection will be closed.</param>
    public static void CloseInMemoryDatabaseConnection(this DbContext database)
    {
        database.Database.GetDbConnection().Close();
    }

    /// <summary>
    /// Closes the database connections used by the in-memory databases.
    /// </summary>
    /// <param name="databases">The databases which connections will be closed.</param>
    public static void CloseInMemoryDatabaseConnections(params DbContext[] databases)
    {
        foreach (var database in databases)
        {
            database.Database.GetDbConnection().Close();
        }
    }
}