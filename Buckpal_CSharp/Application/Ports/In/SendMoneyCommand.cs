using System.ComponentModel.DataAnnotations;
using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Application.Ports.In;

public record SendMoneyCommand
{
    [Required]
    public AccountId SourceAccountId { get; init; }
    
    [Required]
    public AccountId TargetAccountId { get; init; }
    
    [Required]
    public Money Money { get; init; }

    public SendMoneyCommand(AccountId sourceAccountId, AccountId targetAccountId, Money money)
    {
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Money = money;
        
        // Simple manual validation as record class
        if (money.Amount <= 0)
        {
            throw new ArgumentException("Money must be positive");
        }
    }
}
