namespace Buckpal_CSharp.Adapters.Out.Persistence;

/// <summary>
/// 對應 Java 的 SpringDataAccountRepository（JpaRepository&lt;AccountJpaEntity, Long&gt;）
/// </summary>
public interface IAccountRepository
{
    AccountJpaEntity? FindById(long id);
    void Save(AccountJpaEntity account);
}
