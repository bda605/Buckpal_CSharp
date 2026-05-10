using System;

namespace Buckpal_CSharp.Domain.Model;

public class Account
{
    public AccountId? Id { get; }
    public Money BaselineBalance { get; }
    public ActivityWindow ActivityWindow { get; }

    public Account(AccountId id, Money baselineBalance, ActivityWindow activityWindow)
    {
        Id = id;
        BaselineBalance = baselineBalance;
        ActivityWindow = activityWindow;
    }

    public static Account WithoutId(Money baselineBalance, ActivityWindow activityWindow)
    {
        return new Account(null, baselineBalance, activityWindow);
    }

    public static Account WithId(AccountId accountId, Money baselineBalance, ActivityWindow activityWindow)
    {
        return new Account(accountId, baselineBalance, activityWindow);
    }

    public Money CalculateBalance()
    {
        return Money.Add(BaselineBalance, ActivityWindow.CalculateBalance(Id!));
    }

    public bool Withdraw(Money money, AccountId targetAccountId)
    {
        if (!MayWithdraw(money))
        {
            return false;
        }

        var withdrawal = new Activity(
            Id!, Id!, targetAccountId, DateTime.Now, money);
        ActivityWindow.AddActivity(withdrawal);
        return true;
    }

    private bool MayWithdraw(Money money)
    {
        return Money.Subtract(CalculateBalance(), money).IsPositiveOrZero();
    }

    public bool Deposit(Money money, AccountId sourceAccountId)
    {
        var deposit = new Activity(
            Id!, sourceAccountId, Id!, DateTime.Now, money);
        ActivityWindow.AddActivity(deposit);
        return true;
    }
}
