namespace SqliteMemoryDatabaseProvider;

/// <summary>
/// Exception thrown when the in-memory database wasn't able to be created. See inner exception for more details.
/// </summary>
[Serializable]
public class DatabaseCreationException : Exception
{
    /// <summary>
    /// Constructor with a <see cref="message"/> and <see cref="innerException"/>
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception with more information on what failed.</param>
    public DatabaseCreationException(string message, Exception innerException) : base(message, innerException) {}
}