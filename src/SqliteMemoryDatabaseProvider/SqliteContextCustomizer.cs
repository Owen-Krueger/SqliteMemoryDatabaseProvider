using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SqliteMemoryDatabaseProvider;

/// <summary>
/// Provides value converters for SQLite types.
/// These converters include converting DateTimeOffsets, Decimals, and TimeSpans, as SQLite doesn't support these types.
/// </summary>
internal class SqliteContextCustomizer : RelationalModelCustomizer
{
    /// <summary>
    /// Instantiates a new instance of <see cref="SqliteContextCustomizer"/>.
    /// </summary>
    public SqliteContextCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies) { }

    /// <summary>
    /// Extends the context's model builder, adding value converters for DateTimeOffset, Decimal, and TimeSpan types.
    /// </summary>
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            AddConverter<DateTimeOffset, long>(entityType, modelBuilder);
            AddConverter<decimal, double>(entityType, modelBuilder);
            AddConverter<TimeSpan, long>(entityType, modelBuilder);
        }
    }

    /// <summary>
    /// Adds converters from <see cref="TConvertFrom"/> to <see cref="TConvertTo"/> if a converter doesn't already exist.
    /// </summary>
    /// <param name="entityType">Entity containing properties to add converters to.</param>
    /// <param name="modelBuilder">Model builder configuration.</param>
    /// <typeparam name="TConvertFrom">Type to convert when going from the database.</typeparam>
    /// <typeparam name="TConvertTo">Type to convert when going to the database.</typeparam>
    private static void AddConverter<TConvertFrom, TConvertTo>(IMutableEntityType entityType, ModelBuilder modelBuilder)
    {
        var properties = entityType
            .GetProperties()
            .Where(x => 
                x.PropertyInfo?.PropertyType == typeof(TConvertFrom) || 
                x.PropertyInfo?.PropertyType == typeof(TConvertFrom?));
        foreach (var property in properties)
        {
            modelBuilder
                .Entity(entityType.Name)
                .Property(property.Name)
                .HasConversion<TConvertTo>();
        }
    }
}