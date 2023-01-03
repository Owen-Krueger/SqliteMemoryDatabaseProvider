using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.SqliteMemoryDatabaseProvider;

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
    /// Extends the context's model builder, adding value converters for DateTimeOffset types.
    /// </summary>
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            AddDateConverters(entityType, modelBuilder);
            AddDecimalConverters(entityType, modelBuilder);
            AddTimespanConverters(entityType, modelBuilder);
        }
    }

    /// <summary>
    /// Adds value converters for <see cref="DateTimeOffset"/> types. 
    /// </summary>
    private static void AddDateConverters(IMutableEntityType entityType, ModelBuilder modelBuilder)
    {
        var properties = entityType
            .GetProperties()
            .Where(x => 
                x.PropertyInfo.PropertyType == typeof(DateTimeOffset) || 
                x.PropertyInfo.PropertyType == typeof(DateTimeOffset?));
        foreach (var property in properties)
        {
            modelBuilder
                .Entity(entityType.Name)
                .Property(property.Name)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
        }
    }
    
    /// <summary>
    /// Adds value converters for <see cref="decimal"/> types. 
    private static void AddDecimalConverters(IMutableEntityType entityType, ModelBuilder modelBuilder)
    {
        var properties = entityType
            .GetProperties()
            .Where(x => 
                x.PropertyInfo.PropertyType == typeof(decimal) || 
                x.PropertyInfo.PropertyType == typeof(decimal?));
        foreach (var property in properties)
        {
            modelBuilder
                .Entity(entityType.Name)
                .Property(property.Name)
                .HasConversion<double>();
        }
    }
    
    /// <summary>
    /// Adds value converters for <see cref="TimeSpan"/> types. 
    /// </summary>
    private static void AddTimespanConverters(IMutableEntityType entityType, ModelBuilder modelBuilder)
    {
        var properties = entityType
            .GetProperties()
            .Where(x => 
                x.PropertyInfo.PropertyType == typeof(TimeSpan) || 
                x.PropertyInfo.PropertyType == typeof(TimeSpan?));
        foreach (var property in properties)
        {
            modelBuilder
                .Entity(entityType.Name)
                .Property(property.Name)
                .HasConversion(new TimeSpanToStringConverter());
        }
    }
}