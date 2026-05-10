using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Domain.Model;
using FluentAssertions;
using Xunit;

namespace Buckpal_CSharp_Test.Application.Port.In;

/// <summary>
/// SendMoneyCommand 輸入驗證單元測試（對應 Java SendMoneyCommandTest）。
///
/// 與 Java 版本的差異說明：
///   - Java 使用 Jakarta Bean Validation（@NotNull、@PositiveMoney），
///     驗證失敗拋出 ConstraintViolationException。
///   - C# 版本在建構子內手動驗證金額，驗證失敗拋出 ArgumentException。
///   - C# 版本未對 null AccountId 做保護，故省略 null accountId 測試案例。
/// </summary>
public class SendMoneyCommandTest
{
    /// <summary>
    /// 【驗證通過】
    /// 正數金額（10）搭配合法帳戶 ID，應成功建立 Command，不拋出任何例外。
    /// 對應 Java：validationOk()
    /// </summary>
    [Fact]
    public void ValidationOk()
    {
        // Arrange & Act
        var act = () => new SendMoneyCommand(
            new AccountId(42L),
            new AccountId(43L),
            Money.Of(10L));

        // Assert
        act.Should().NotThrow("正數金額與合法帳戶 ID 應可成功建立 Command");
    }

    /// <summary>
    /// 【金額驗證失敗 — 負數】
    /// 負數金額（-10）應拋出 ArgumentException，
    /// 訊息應包含 "positive" 關鍵字。
    /// 對應 Java：moneyValidationFails()（Java 拋 ConstraintViolationException）
    /// </summary>
    [Fact]
    public void MoneyValidationFails_NegativeAmount()
    {
        // Arrange & Act
        var act = () => new SendMoneyCommand(
            new AccountId(42L),
            new AccountId(43L),
            Money.Of(-10L));

        // Assert
        act.Should().Throw<ArgumentException>(
            "負數金額不合法，應拋出 ArgumentException")
           .WithMessage("*positive*");
    }

    /// <summary>
    /// 【金額驗證失敗 — 零元】
    /// 零元金額（0）也屬於非正數，應拋出 ArgumentException。
    /// C# 版本的驗證條件為 amount &lt;= 0（嚴格要求正數），
    /// 對應 Java @PositiveMoney 只允許嚴格正數的語意。
    /// </summary>
    [Fact]
    public void MoneyValidationFails_ZeroAmount()
    {
        // Arrange & Act
        var act = () => new SendMoneyCommand(
            new AccountId(42L),
            new AccountId(43L),
            Money.Of(0L));

        // Assert
        act.Should().Throw<ArgumentException>(
            "零元也是非正數，應拋出 ArgumentException");
    }
}
