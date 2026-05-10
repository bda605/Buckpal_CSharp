using Buckpal_CSharp.Domain.Model;
using FluentAssertions;
using Xunit;
using static Buckpal_CSharp_Test.Common.ActivityTestData;

namespace Buckpal_CSharp_Test.Application.Domain;

/// <summary>
/// ActivityWindow 領域模型單元測試（對應 Java ActivityWindowTest）。
/// 測試範圍：起始時間戳記、結束時間戳記、帳戶餘額計算。
/// </summary>
public class ActivityWindowTest
{
    /// <summary>
    /// 【起始時間戳記】
    /// 窗口應從三筆活動中找出最早的時間戳記（2019-08-03）。
    /// 對應 Java：calculatesStartTimestamp()
    /// </summary>
    [Fact]
    public void CalculatesStartTimestamp()
    {
        // Arrange
        var window = new ActivityWindow(
            DefaultActivity().WithTimestamp(StartDate()).Build(),
            DefaultActivity().WithTimestamp(InBetweenDate()).Build(),
            DefaultActivity().WithTimestamp(EndDate()).Build());

        // Act & Assert
        window.StartTimestamp.Should().Be(StartDate(),
            "三筆活動中最早的時間戳記應為 2019-08-03");
    }

    /// <summary>
    /// 【結束時間戳記】
    /// 窗口應從三筆活動中找出最晚的時間戳記（2019-08-05）。
    /// 對應 Java：calculatesEndTimestamp()
    /// </summary>
    [Fact]
    public void CalculatesEndTimestamp()
    {
        // Arrange
        var window = new ActivityWindow(
            DefaultActivity().WithTimestamp(StartDate()).Build(),
            DefaultActivity().WithTimestamp(InBetweenDate()).Build(),
            DefaultActivity().WithTimestamp(EndDate()).Build());

        // Act & Assert
        window.EndTimestamp.Should().Be(EndDate(),
            "三筆活動中最晚的時間戳記應為 2019-08-05");
    }

    /// <summary>
    /// 【帳戶餘額計算】
    /// account1 匯出 999 + 1 = 1000，account2 匯回 500 給 account1；
    /// 因此 account1 淨額 = 500 - 1000 = -500，account2 淨額 = 1000 - 500 = 500。
    /// 對應 Java：calculatesBalance()
    /// </summary>
    [Fact]
    public void CalculatesBalance()
    {
        // Arrange
        var account1 = new AccountId(1L);
        var account2 = new AccountId(2L);

        var window = new ActivityWindow(
            // account1 → account2，金額 999
            DefaultActivity()
                .WithSourceAccount(account1).WithTargetAccount(account2)
                .WithMoney(Money.Of(999L)).Build(),
            // account1 → account2，金額 1
            DefaultActivity()
                .WithSourceAccount(account1).WithTargetAccount(account2)
                .WithMoney(Money.Of(1L)).Build(),
            // account2 → account1，金額 500
            DefaultActivity()
                .WithSourceAccount(account2).WithTargetAccount(account1)
                .WithMoney(Money.Of(500L)).Build());

        // Act & Assert
        window.CalculateBalance(account1).Should().Be(Money.Of(-500m),
            "account1 存入 500，提出 1000，淨額 = -500");
        window.CalculateBalance(account2).Should().Be(Money.Of(500m),
            "account2 存入 1000，提出 500，淨額 = +500");
    }

    // ── 輔助日期方法 ───────────────────────────────────────────────────────────

    private static DateTime StartDate()     => new DateTime(2019, 8, 3);
    private static DateTime InBetweenDate() => new DateTime(2019, 8, 4);
    private static DateTime EndDate()       => new DateTime(2019, 8, 5);
}
