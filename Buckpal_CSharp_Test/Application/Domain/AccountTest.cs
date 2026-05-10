using Buckpal_CSharp.Domain.Model;
using FluentAssertions;
using Xunit;
using static Buckpal_CSharp_Test.Common.AccountTestData;
using static Buckpal_CSharp_Test.Common.ActivityTestData;

namespace Buckpal_CSharp_Test.Application.Domain;

/// <summary>
/// Account 領域模型單元測試（對應 Java AccountTest）。
/// 測試範圍：餘額計算、提款成功、提款失敗、存款成功。
/// </summary>
public class AccountTest
{
    /// <summary>
    /// 【餘額計算】
    /// 基準餘額 555 加上兩筆存入活動（999 + 1），總餘額應為 1555。
    /// 對應 Java：calculatesBalance()
    /// </summary>
    [Fact]
    public void CalculatesBalance()
    {
        // Arrange
        var accountId = new AccountId(1L);
        var account = DefaultAccount()
            .WithAccountId(accountId)
            .WithBaselineBalance(Money.Of(555L))
            .WithActivityWindow(new ActivityWindow(
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(999L)).Build(),
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(1L)).Build()))
            .Build();

        // Act
        var balance = account.CalculateBalance();

        // Assert
        balance.Should().Be(Money.Of(1555m));
    }

    /// <summary>
    /// 【提款成功】
    /// 帳戶餘額 1555，提款 555 應成功（回傳 true），
    /// 活動筆數由 2 增為 3，剩餘餘額應為 1000。
    /// 對應 Java：withdrawalSucceeds()
    /// </summary>
    [Fact]
    public void WithdrawalSucceeds()
    {
        // Arrange
        var accountId = new AccountId(1L);
        var account = DefaultAccount()
            .WithAccountId(accountId)
            .WithBaselineBalance(Money.Of(555L))
            .WithActivityWindow(new ActivityWindow(
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(999L)).Build(),
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(1L)).Build()))
            .Build();

        // Act
        bool success = account.Withdraw(Money.Of(555L), new AccountId(99L));

        // Assert
        success.Should().BeTrue("餘額足夠，提款應成功");
        account.ActivityWindow.Activities.Should().HaveCount(3, "提款後應新增一筆提款活動");
        account.CalculateBalance().Should().Be(Money.Of(1000m), "1555 - 555 = 1000");
    }

    /// <summary>
    /// 【提款失敗】
    /// 帳戶餘額 1555，嘗試提款 1556 超過餘額應失敗（回傳 false），
    /// 活動筆數維持 2，餘額不變仍為 1555。
    /// 對應 Java：withdrawalFailure()
    /// </summary>
    [Fact]
    public void WithdrawalFailure()
    {
        // Arrange
        var accountId = new AccountId(1L);
        var account = DefaultAccount()
            .WithAccountId(accountId)
            .WithBaselineBalance(Money.Of(555L))
            .WithActivityWindow(new ActivityWindow(
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(999L)).Build(),
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(1L)).Build()))
            .Build();

        // Act
        bool success = account.Withdraw(Money.Of(1556L), new AccountId(99L));

        // Assert
        success.Should().BeFalse("超過餘額時提款應失敗");
        account.ActivityWindow.Activities.Should().HaveCount(2, "提款失敗不應新增活動記錄");
        account.CalculateBalance().Should().Be(Money.Of(1555m), "提款失敗後餘額不應改變");
    }

    /// <summary>
    /// 【存款成功】
    /// 帳戶餘額 1555，存入 445 應成功（回傳 true），
    /// 活動筆數由 2 增為 3，總餘額應增至 2000。
    /// 對應 Java：depositSuccess()
    /// </summary>
    [Fact]
    public void DepositSuccess()
    {
        // Arrange
        var accountId = new AccountId(1L);
        var account = DefaultAccount()
            .WithAccountId(accountId)
            .WithBaselineBalance(Money.Of(555L))
            .WithActivityWindow(new ActivityWindow(
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(999L)).Build(),
                DefaultActivity().WithTargetAccount(accountId).WithMoney(Money.Of(1L)).Build()))
            .Build();

        // Act
        bool success = account.Deposit(Money.Of(445L), new AccountId(99L));

        // Assert
        success.Should().BeTrue("存款永遠應成功");
        account.ActivityWindow.Activities.Should().HaveCount(3, "存款後應新增一筆存款活動");
        account.CalculateBalance().Should().Be(Money.Of(2000m), "1555 + 445 = 2000");
    }
}
