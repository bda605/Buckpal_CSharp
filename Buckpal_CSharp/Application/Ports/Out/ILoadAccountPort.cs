using System;
using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Application.Ports.Out;

public interface ILoadAccountPort
{
    Account LoadAccount(AccountId accountId, DateTime baselineDate);
}
