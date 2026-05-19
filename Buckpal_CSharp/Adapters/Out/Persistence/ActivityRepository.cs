namespace Buckpal_CSharp.Adapters.Out.Persistence;

/// <summary>
/// 對應 Java 的 ActivityRepository，使用 EF Core 實作三個自訂查詢
/// </summary>
public class ActivityRepository : IActivityRepository
{
    private readonly BuckpalDbContext _dbContext;

    public ActivityRepository(BuckpalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 對應 Java: findByOwnerSince — 查詢指定帳戶自某時間點之後的所有活動
    /// </summary>
    public List<ActivityJpaEntity> FindByOwnerSince(long ownerAccountId, DateTime since)
    {
        return _dbContext.Activities
            .Where(a => a.OwnerAccountId == ownerAccountId && a.Timestamp >= since)
            .ToList();
    }

    /// <summary>
    /// 對應 Java: getDepositBalanceUntil — 計算指定帳戶在某時間點前的存款總額
    /// </summary>
    public decimal GetDepositBalanceUntil(long accountId, DateTime until)
    {
        return _dbContext.Activities
            .Where(a => a.TargetAccountId == accountId
                     && a.OwnerAccountId == accountId
                     && a.Timestamp < until)
            .Sum(a => (decimal?)a.Amount) ?? 0m;
    }

    /// <summary>
    /// 對應 Java: getWithdrawalBalanceUntil — 計算指定帳戶在某時間點前的提款總額
    /// </summary>
    public decimal GetWithdrawalBalanceUntil(long accountId, DateTime until)
    {
        return _dbContext.Activities
            .Where(a => a.SourceAccountId == accountId
                     && a.OwnerAccountId == accountId
                     && a.Timestamp < until)
            .Sum(a => (decimal?)a.Amount) ?? 0m;
    }

    public void Save(ActivityJpaEntity activity)
    {
        _dbContext.Activities.Add(activity);
        _dbContext.SaveChanges();
    }
}
