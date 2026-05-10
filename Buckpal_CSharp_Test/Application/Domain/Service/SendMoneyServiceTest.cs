using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Model;
using Buckpal_CSharp.Domain.Service;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Buckpal_CSharp_Test.Application.Domain.Service;

/// <summary>
/// SendMoneyService 應用服務單元測試（對應 Java SendMoneyServiceTest）。
///
/// 與 Java 版本的差異說明：
///   - Java 使用 Mockito.mock(Account.class) 模擬 Account，
///     因為 Java 的 Account.withdraw / Account.deposit 可被 Mockito 攔截。
///   - C# 的 Account.Withdraw / Account.Deposit 為 non-virtual，無法用 Moq 模擬，
///     因此改用真實 Account 物件，透過控制初始餘額來觸發成功或失敗情境。
///   - Java 版本驗證 AccountLock 的鎖定/釋放行為；
///     C# 版本的 SendMoneyService 未實作 IAccountLock，故省略鎖定行為驗證。
///
/// Mock 對象：ILoadAccountPort、IUpdateAccountStatePort（使用 Moq）
/// 真實對象：Account（由工廠方法建立，含真實業務邏輯）
/// </summary>
public class SendMoneyServiceTest
{
    private readonly ILoadAccountPort        _loadAccountPortMock        = Substitute.For<ILoadAccountPort>();
    private readonly IUpdateAccountStatePort _updateAccountStatePortMock = Substitute.For<IUpdateAccountStatePort>();
    private readonly SendMoneyService        _sendMoneyService;

    public SendMoneyServiceTest()
    {
        // SendMoneyService 建構子接受 ILoadAccountPort、IUpdateAccountStatePort 兩個相依
        _sendMoneyService = new SendMoneyService(
            _loadAccountPortMock,
            _updateAccountStatePortMock);
    }

    /// <summary>
    /// 【提款失敗情境】
    /// 來源帳戶餘額為 0，嘗試提款 100 必然失敗，
    /// Service 應回傳 false，且不應呼叫 UpdateActivities 更新任何帳戶狀態。
    ///
    /// 對應 Java：givenWithdrawalFails_thenOnlySourceAccountIsLockedAndReleased()
    /// （Java 額外驗證 AccountLock 行為，C# 省略）
    /// </summary>
    [Fact]
    public void GivenWithdrawalFails_ReturnsFalse_AndActivitiesNotUpdated()
    {
        // Arrange：來源帳戶餘額 0，提款 100 必然失敗
        var sourceAccountId = new AccountId(41L);
        var targetAccountId = new AccountId(42L);

        var sourceAccount = Account.WithId(sourceAccountId, Money.Of(0L), new ActivityWindow());
        var targetAccount = Account.WithId(targetAccountId, Money.Of(500L), new ActivityWindow());

        _loadAccountPortMock
            .LoadAccount(sourceAccountId, Arg.Any<DateTime>())
            .Returns(sourceAccount);
        _loadAccountPortMock
            .LoadAccount(targetAccountId, Arg.Any<DateTime>())
            .Returns(targetAccount);

        var command = new SendMoneyCommand(sourceAccountId, targetAccountId, Money.Of(100L));

        // Act
        bool success = _sendMoneyService.SendMoney(command);

        // Assert
        success.Should().BeFalse("來源帳戶餘額不足時轉帳應失敗");

        _updateAccountStatePortMock.DidNotReceive().UpdateActivities(Arg.Any<Account>());
    }

    /// <summary>
    /// 【轉帳成功情境】
    /// 來源帳戶餘額 1000，轉帳 500 給目標帳戶，
    /// Service 應回傳 true，並呼叫 UpdateActivities 分別更新來源與目標帳戶；
    /// 最終來源帳戶餘額 500，目標帳戶餘額 500。
    ///
    /// 對應 Java：transactionSucceeds()
    /// </summary>
    [Fact]
    public void TransactionSucceeds()
    {
        // Arrange：來源帳戶餘額 1000，目標帳戶餘額 0，轉帳 500
        var sourceAccountId = new AccountId(41L);
        var targetAccountId = new AccountId(42L);
        var transferAmount   = Money.Of(500L);

        var sourceAccount = Account.WithId(sourceAccountId, Money.Of(1000L), new ActivityWindow());
        var targetAccount = Account.WithId(targetAccountId, Money.Of(0L),    new ActivityWindow());

        _loadAccountPortMock
            .LoadAccount(sourceAccountId, Arg.Any<DateTime>())
            .Returns(sourceAccount);
        _loadAccountPortMock
            .LoadAccount(targetAccountId, Arg.Any<DateTime>())
            .Returns(targetAccount);

        var command = new SendMoneyCommand(sourceAccountId, targetAccountId, transferAmount);

        // Act
        bool success = _sendMoneyService.SendMoney(command);

        // Assert — 轉帳結果
        success.Should().BeTrue("餘額充足時轉帳應成功");

        // Assert — 來源帳戶狀態已被持久化
        _updateAccountStatePortMock.Received(1).UpdateActivities(sourceAccount);

        // Assert — 目標帳戶狀態已被持久化
        _updateAccountStatePortMock.Received(1).UpdateActivities(targetAccount);

        // Assert — 轉帳後餘額正確（驗證領域邏輯確實執行）
        sourceAccount.CalculateBalance().Should().Be(Money.Of(500m),
            "來源帳戶：1000 - 500 = 500");
        targetAccount.CalculateBalance().Should().Be(Money.Of(500m),
            "目標帳戶：0 + 500 = 500");
    }
}
