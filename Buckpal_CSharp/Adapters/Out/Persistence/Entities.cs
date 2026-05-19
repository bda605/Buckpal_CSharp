using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buckpal_CSharp.Adapters.Out.Persistence;

[Table("account")]
public class AccountJpaEntity
{
    [Key]
    public long Id { get; set; }
}

[Table("activity")]
public class ActivityJpaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public DateTime Timestamp { get; set; }
    
    public long OwnerAccountId { get; set; }
    public long SourceAccountId { get; set; }
    public long TargetAccountId { get; set; }
    
    public decimal Amount { get; set; }
}
