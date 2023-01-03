using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.SqliteMemoryDatabaseProvider.UnitTests;

[Table("TestTable")]
internal class TestModel
{
    [Key]
    public long ModelId { get; set; }

    public string OtherProperty { get; set; }
    
    public DateTimeOffset Date { get; set; }

    public decimal Decimal { get; set; }
    
    public TimeSpan TimeSpan { get; set; }
}