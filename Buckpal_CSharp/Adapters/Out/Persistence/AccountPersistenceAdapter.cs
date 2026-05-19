using System;
using System.Linq;
using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Buckpal_CSharp.Adapters.Out.Persistence;

public class AccountPersistenceAdapter : ILoadAccountPort, IUpdateAccountStatePort
{
    private readonly BuckpalDbContext _dbContext;

    public AccountPersistenceAdapter(BuckpalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Account LoadAccount(AccountId accountId, DateTime baselineDate)
    {
        var accountEntity = _dbContext.Accounts.Find(accountId.Value) 
                            ?? throw new Exception("Account not found");

        var activities = _dbContext.Activities
            .Where(a => a.OwnerAccountId == accountId.Value && a.Timestamp >= baselineDate)
            .ToList();

        var withdrawalBalance = _dbContext.Activities
            .Where(a => a.SourceAccountId == accountId.Value && a.Timestamp < baselineDate)
            .Sum(a => a.Amount);

        var depositBalance = _dbContext.Activities
            .Where(a => a.TargetAccountId == accountId.Value && a.Timestamp < baselineDate)
            .Sum(a => a.Amount);

        var baselineBalance = depositBalance - withdrawalBalance;

        var mappedActivities = activities.Select(a => new Activity(
            new ActivityId(a.Id),
            new AccountId(a.OwnerAccountId),
            new AccountId(a.SourceAccountId),
            new AccountId(a.TargetAccountId),
            a.Timestamp,
            Money.Of(a.Amount)
        )).ToList();

        return Account.WithId(
            accountId,
            Money.Of(baselineBalance),
            new ActivityWindow(mappedActivities)
        );
    }

    public void UpdateActivities(Account account)
    {
        foreach (var activity in account.ActivityWindow.Activities.Where(a => a.Id == null))
        {
            var entity = new ActivityJpaEntity
            {
                OwnerAccountId = activity.OwnerAccountId.Value,
                SourceAccountId = activity.SourceAccountId.Value,
                TargetAccountId = activity.TargetAccountId.Value,
                Timestamp = activity.Timestamp,
                Amount = activity.Money.Amount
            };

            _dbContext.Activities.Add(entity);
        }

        _dbContext.SaveChanges();
    }
}
