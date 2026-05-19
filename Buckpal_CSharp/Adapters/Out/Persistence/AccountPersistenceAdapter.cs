using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Adapters.Out.Persistence;

public class AccountPersistenceAdapter : ILoadAccountPort, IUpdateAccountStatePort
{
    private readonly IAccountRepository _accountRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly AccountMapper _accountMapper;

    public AccountPersistenceAdapter(
        IAccountRepository accountRepository,
        IActivityRepository activityRepository,
        AccountMapper accountMapper)
    {
        _accountRepository = accountRepository;
        _activityRepository = activityRepository;
        _accountMapper = accountMapper;
    }

    public Account LoadAccount(AccountId accountId, DateTime baselineDate)
    {
        var accountEntity = _accountRepository.FindById(accountId.Value)
            ?? throw new Exception("Account not found");

        var activities = _activityRepository.FindByOwnerSince(accountId.Value, baselineDate);

        var withdrawalBalance = _activityRepository.GetWithdrawalBalanceUntil(accountId.Value, baselineDate);
        var depositBalance = _activityRepository.GetDepositBalanceUntil(accountId.Value, baselineDate);

        return _accountMapper.MapToDomainEntity(accountEntity, activities, withdrawalBalance, depositBalance);
    }

    public void UpdateActivities(Account account)
    {
        foreach (var activity in account.ActivityWindow.Activities.Where(a => a.Id == null))
        {
            _activityRepository.Save(_accountMapper.MapToJpaEntity(activity));
        }
    }
}
