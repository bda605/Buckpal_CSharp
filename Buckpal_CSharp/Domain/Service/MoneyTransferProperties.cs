using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Domain.Service;

/// <summary>
/// 轉帳功能的設定屬性（最大轉帳金額上限）
/// </summary>
public class MoneyTransferProperties
{
    public Money MaximumTransferThreshold { get; set; } = Money.Of(1_000_000m);
}
