using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Application.Ports.Out;

public interface IUpdateAccountStatePort
{
    void UpdateActivities(Account account);
}
