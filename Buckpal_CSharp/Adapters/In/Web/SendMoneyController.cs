using Microsoft.AspNetCore.Mvc;
using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Domain.Model;

namespace Buckpal_CSharp.Adapters.In.Web;

[ApiController]
[Route("[controller]")]
public class SendMoneyController : ControllerBase
{
    private readonly ISendMoneyUseCase _sendMoneyUseCase;

    public SendMoneyController(ISendMoneyUseCase sendMoneyUseCase)
    {
        _sendMoneyUseCase = sendMoneyUseCase;
    }

    [HttpPost("send")]
    public IActionResult SendMoney(
        [FromQuery] long sourceAccountId,
        [FromQuery] long targetAccountId,
        [FromQuery] decimal amount)
    {
        var command = new SendMoneyCommand(
            new AccountId(sourceAccountId),
            new AccountId(targetAccountId),
            Money.Of(amount));

        if (_sendMoneyUseCase.SendMoney(command))
        {
            return Ok(new { message = "Transfer successful" });
        }

        return BadRequest(new { message = "Transfer failed" });
    }
}
