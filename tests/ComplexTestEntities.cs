using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SqliteMemoryDatabaseProvider.UnitTests;

public interface IDateTimeConverter
{
    public DateTime ConvertToDateTime(DateTimeOffset date);

    public DateTimeOffset ConvertToDateTimeOffset(DateTime date);
}

[Table("Table", Schema = "Schema")]
public class TestModelWithDate
{
    public int Id { get; set; }

    public DateTimeOffset Date { get; set; }
}

internal interface IComplexTestEntities
{
    public DbSet<TestModelWithDate> TestModels { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

internal class ComplexTestEntities : DbContext, IComplexTestEntities
{
    public DbSet<TestModelWithDate> TestModels { get; set; }

    private readonly IDateTimeConverter dateTimeConverter;

    public ComplexTestEntities(DbContextOptions<ComplexTestEntities> dbContextOptions, IDateTimeConverter dateTimeConverter) : base(dbContextOptions) { this.dateTimeConverter = dateTimeConverter; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestModelWithDate>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<TestModelWithDate>()
            .Property(x => x.Date)
            .HasConversion(x => dateTimeConverter.ConvertToDateTime(x),
                x => dateTimeConverter.ConvertToDateTimeOffset(x));
    }
}