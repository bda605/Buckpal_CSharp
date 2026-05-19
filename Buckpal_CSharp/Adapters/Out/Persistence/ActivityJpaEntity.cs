using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buckpal_CSharp.Adapters.Out.Persistence;

[Table("activity")]
public class ActivityJpaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column]
    public DateTime Timestamp { get; set; }

    [Column]
    public long OwnerAccountId { get; set; }

    [Column]
    public long SourceAccountId { get; set; }

    [Column]
    public long TargetAccountId { get; set; }

    [Column]
    public decimal Amount { get; set; }
}
