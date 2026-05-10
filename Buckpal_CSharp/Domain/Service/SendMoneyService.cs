using System;
using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Application.Ports.Out;

namespace Buckpal_CSharp.Domain.Service;

public class SendMoneyService : ISendMoneyUseCase
{
    private readonly ILoadAccountPort _loadAccountPort;
    private readonly IUpdateAccountStatePort _updateAccountStatePort;

    public SendMoneyService(
        ILoadAccountPort loadAccountPort, 
        IUpdateAccountStatePort updateAccountStatePort)
    {
        _loadAccountPort = loadAccountPort;
        _updateAccountStatePort = updateAccountStatePort;
    }

    public bool SendMoney(SendMoneyCommand command)
    {
        // Simple lock mechanism can be added here
        
        var baselineDate = DateTime.Now.AddDays(-10);
        
        var sourceAccount = _loadAccountPort.LoadAccount(command.SourceAccountId, baselineDate);
        var targetAccount = _loadAccountPort.LoadAccount(command.TargetAccountId, baselineDate);

        if (!sourceAccount.Withdraw(command.Money, targetAccount.Id!))
        {
            return false;
        }

        if (!targetAccount.Deposit(command.Money, sourceAccount.Id!))
        {
            return false;
        }

        _updateAccountStatePort.UpdateActivities(sourceAccount);
        _updateAccountStatePort.UpdateActivities(targetAccount);

        return true;
    }
}
