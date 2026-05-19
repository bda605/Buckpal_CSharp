using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Domain.Service;

public class NoOpAccountLock : IAccountLock
{
    public void LockAccount(AccountId accountId)
    {
        // 不做任何事（NoOp）
    }

    public void ReleaseAccount(AccountId accountId)
    {
        // 不做任何事（NoOp）
    }
}
