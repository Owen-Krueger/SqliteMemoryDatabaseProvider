using Microsoft.EntityFrameworkCore;

namespace EntityFramework.SqliteMemoryDatabaseProvider.UnitTests;

internal interface ITestEntities
{
    public DbSet<TestModel> TestModels { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

internal class TestEntities : DbContext, ITestEntities
{
    public DbSet<TestModel> TestModels { get; set; }

    public TestEntities(DbContextOptions<TestEntities> dbContextOptions) : base(dbContextOptions) { }
}