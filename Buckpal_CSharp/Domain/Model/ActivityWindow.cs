using System;
using System.Collections.Generic;
using System.Linq;

namespace Buckpal_CSharp.Domain.Model;

public class ActivityWindow
{
    private readonly List<Activity> _activities;

    public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();

    public ActivityWindow(params Activity[] activities)
    {
        _activities = new List<Activity>(activities);
    }

    public ActivityWindow(IEnumerable<Activity> activities)
    {
        _activities = new List<Activity>(activities);
    }

    public DateTime StartTimestamp => _activities.Min(a => a.Timestamp);
    public DateTime EndTimestamp => _activities.Max(a => a.Timestamp);

    public Money CalculateBalance(AccountId accountId)
    {
        var depositBalance = _activities
            .Where(a => a.TargetAccountId == accountId)
            .Select(a => a.Money)
            .Aggregate(Money.Zero(), Money.Add);

        var withdrawalBalance = _activities
            .Where(a => a.SourceAccountId == accountId)
            .Select(a => a.Money)
            .Aggregate(Money.Zero(), Money.Add);

        return Money.Add(depositBalance, withdrawalBalance.Negate());
    }

    public void AddActivity(Activity activity)
    {
        _activities.Add(activity);
    }
}
