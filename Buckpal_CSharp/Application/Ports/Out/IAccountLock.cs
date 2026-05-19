using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Application.Ports.Out;

public interface IAccountLock
{
    void LockAccount(AccountId accountId);
    void ReleaseAccount(AccountId accountId);
}
