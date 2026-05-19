namespace Buckpal_CSharp.Adapters.Out.Persistence;

/// <summary>
/// 對應 Java 的 ActivityRepository（JpaRepository&lt;ActivityJpaEntity, Long&gt; 含自訂查詢）
/// </summary>
public interface IActivityRepository
{
    List<ActivityJpaEntity> FindByOwnerSince(long ownerAccountId, DateTime since);
    decimal GetDepositBalanceUntil(long accountId, DateTime until);
    decimal GetWithdrawalBalanceUntil(long accountId, DateTime until);
    void Save(ActivityJpaEntity activity);
}
