using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp_Test.Common;

/// <summary>
/// 測試用 Account 資料建構器（對應 Java AccountTestData）。
/// 預設值：accountId=42, baselineBalance=999,
/// activityWindow 含兩筆 DefaultActivity（source=42, target=41, money=999）。
/// </summary>
public static class AccountTestData
{
    /// <summary>建立含預設值的 AccountBuilder，方便測試覆蓋所需欄位。</summary>
    public static AccountBuilder DefaultAccount() =>
        new AccountBuilder()
            .WithAccountId(new AccountId(42L))
            .WithBaselineBalance(Money.Of(999L))
            .WithActivityWindow(new ActivityWindow(
                ActivityTestData.DefaultActivity().Build(),
                ActivityTestData.DefaultActivity().Build()));

    public class AccountBuilder
    {
        private AccountId?     _accountId       = null;
        private Money          _baselineBalance  = Money.Zero();
        private ActivityWindow _activityWindow   = new ActivityWindow();

        public AccountBuilder WithAccountId(AccountId id)         { _accountId      = id; return this; }
        public AccountBuilder WithBaselineBalance(Money m)        { _baselineBalance = m;  return this; }
        public AccountBuilder WithActivityWindow(ActivityWindow w) { _activityWindow  = w;  return this; }

        /// <summary>
        /// 依據是否有 AccountId 決定呼叫 WithId 或 WithoutId 工廠方法，
        /// 對應 Java Account.withId / Account.withoutId。
        /// </summary>
        public Account Build() =>
            _accountId is not null
                ? Account.WithId(_accountId, _baselineBalance, _activityWindow)
                : Account.WithoutId(_baselineBalance, _activityWindow);
    }
}
