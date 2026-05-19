using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Domain.Service;

public class ThresholdExceededException : Exception
{
    public ThresholdExceededException(Money threshold, Money actual)
        : base($"轉帳金額超過上限：嘗試轉帳 {actual.Amount}，但上限為 {threshold.Amount}！")
    {
    }
}
