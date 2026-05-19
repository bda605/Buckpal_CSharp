using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Domain.Service;

public class GetAccountBalanceService : IGetAccountBalanceQuery
{
    private readonly ILoadAccountPort _loadAccountPort;

    public GetAccountBalanceService(ILoadAccountPort loadAccountPort)
    {
        _loadAccountPort = loadAccountPort;
    }

    public Money GetAccountBalance(AccountId accountId)
    {
        return _loadAccountPort
            .LoadAccount(accountId, DateTime.Now)
            .CalculateBalance();
    }
}
