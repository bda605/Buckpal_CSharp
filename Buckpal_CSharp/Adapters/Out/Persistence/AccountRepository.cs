namespace Buckpal_CSharp.Adapters.Out.Persistence;

/// <summary>
/// 對應 Java 的 SpringDataAccountRepository，使用 EF Core 實作
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly BuckpalDbContext _dbContext;

    public AccountRepository(BuckpalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public AccountJpaEntity? FindById(long id)
    {
        return _dbContext.Accounts.Find(id);
    }

    public void Save(AccountJpaEntity account)
    {
        _dbContext.Accounts.Add(account);
        _dbContext.SaveChanges();
    }
}
