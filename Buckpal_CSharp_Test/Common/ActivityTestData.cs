using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp_Test.Common;

/// <summary>
/// 測試用 Activity 資料建構器（對應 Java ActivityTestData）。
/// 預設值：owner=42, source=42, target=41, money=999, timestamp=DateTime.Now。
/// 使用鏈式 With* 方法覆蓋個別欄位，最後呼叫 Build() 建立物件。
/// </summary>
public static class ActivityTestData
{
    /// <summary>建立含預設值的 ActivityBuilder，方便測試覆蓋所需欄位。</summary>
    public static ActivityBuilder DefaultActivity() =>
        new ActivityBuilder()
            .WithOwnerAccount(new AccountId(42L))
            .WithSourceAccount(new AccountId(42L))
            .WithTargetAccount(new AccountId(41L))
            .WithTimestamp(DateTime.Now)
            .WithMoney(Money.Of(999L));

    public class ActivityBuilder
    {
        private ActivityId? _id;
        private AccountId _ownerAccountId = new(42L);
        private AccountId _sourceAccountId = new(42L);
        private AccountId _targetAccountId = new(41L);
        private DateTime  _timestamp       = DateTime.Now;
        private Money     _money           = Money.Of(999L);

        public ActivityBuilder WithId(ActivityId id)            { _id             = id;        return this; }
        public ActivityBuilder WithOwnerAccount(AccountId id)   { _ownerAccountId = id;        return this; }
        public ActivityBuilder WithSourceAccount(AccountId id)  { _sourceAccountId = id;       return this; }
        public ActivityBuilder WithTargetAccount(AccountId id)  { _targetAccountId = id;       return this; }
        public ActivityBuilder WithTimestamp(DateTime ts)        { _timestamp       = ts;       return this; }
        public ActivityBuilder WithMoney(Money money)            { _money           = money;    return this; }

        /// <summary>
        /// 依據是否有 ID 決定呼叫哪個 Activity 建構子：
        /// 有 ID → 已持久化的活動；無 ID → 尚未持久化的新活動。
        /// </summary>
        public Activity Build() =>
            _id is not null
                ? new Activity(_id, _ownerAccountId, _sourceAccountId, _targetAccountId, _timestamp, _money)
                : new Activity(_ownerAccountId, _sourceAccountId, _targetAccountId, _timestamp, _money);
    }
}
