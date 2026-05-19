using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Application.Ports.In;

public interface IGetAccountBalanceQuery
{
    Money GetAccountBalance(AccountId accountId);
}
