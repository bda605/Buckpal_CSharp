using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Adapters.Out.Persistence;

public class AccountMapper
{
    public Account MapToDomainEntity(
        AccountJpaEntity account,
        List<ActivityJpaEntity> activities,
        decimal withdrawalBalance,
        decimal depositBalance)
    {
        var baselineBalance = Money.Subtract(
            Money.Of(depositBalance),
            Money.Of(withdrawalBalance));

        return Account.WithId(
            new AccountId(account.Id),
            baselineBalance,
            MapToActivityWindow(activities));
    }

    public ActivityWindow MapToActivityWindow(List<ActivityJpaEntity> activities)
    {
        var mappedActivities = activities.Select(a => new Activity(
            new ActivityId(a.Id),
            new AccountId(a.OwnerAccountId),
            new AccountId(a.SourceAccountId),
            new AccountId(a.TargetAccountId),
            a.Timestamp,
            Money.Of(a.Amount)
        )).ToList();

        return new ActivityWindow(mappedActivities);
    }

    public ActivityJpaEntity MapToJpaEntity(Activity activity)
    {
        return new ActivityJpaEntity
        {
            Id = activity.Id?.Value ?? 0,
            Timestamp = activity.Timestamp,
            OwnerAccountId = activity.OwnerAccountId.Value,
            SourceAccountId = activity.SourceAccountId.Value,
            TargetAccountId = activity.TargetAccountId.Value,
            Amount = activity.Money.Amount
        };
    }
}
