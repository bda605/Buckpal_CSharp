using System;

namespace Buckpal_CSharp.Domain.Model;

public record ActivityId(long Value);

public class Activity
{
    public ActivityId? Id { get; }
    public AccountId OwnerAccountId { get; }
    public AccountId SourceAccountId { get; }
    public AccountId TargetAccountId { get; }
    public DateTime Timestamp { get; }
    public Money Money { get; }

    public Activity(
        AccountId ownerAccountId,
        AccountId sourceAccountId,
        AccountId targetAccountId,
        DateTime timestamp,
        Money money)
    {
        Id = null;
        OwnerAccountId = ownerAccountId;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Timestamp = timestamp;
        Money = money;
    }

    public Activity(
        ActivityId id,
        AccountId ownerAccountId,
        AccountId sourceAccountId,
        AccountId targetAccountId,
        DateTime timestamp,
        Money money)
    {
        Id = id;
        OwnerAccountId = ownerAccountId;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Timestamp = timestamp;
        Money = money;
    }
}
