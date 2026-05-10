namespace Buckpal_CSharp.Application.Ports.In;

public interface ISendMoneyUseCase
{
    bool SendMoney(SendMoneyCommand command);
}
