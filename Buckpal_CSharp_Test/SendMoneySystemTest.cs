using System.Net;
using Buckpal_CSharp.Adapters.Out.Persistence;
using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Buckpal_CSharp_Test;

/// <summary>
/// SendMoney 端對端系統測試（對應 Java SendMoneySystemTest）。
///
/// 與 Java 版本的差異說明：
///   - Java 使用 @Sql("SendMoneySystemTest.sql") 透過 SQL 檔案植入測試資料；
///     C# 改為在 WebApplicationFactory 的 ConfigureServices 中以 EF Core 植入。
///   - Java 透過注入 LoadAccountPort 直接查餘額；
///     C# 同樣透過 ILoadAccountPort 服務驗證轉帳前後的餘額變化。
///   - 路由差異：Java 使用 Path Variable（/accounts/send/1/2/500）；
///     C# 使用 Query String（/SendMoney/send?sourceAccountId=1&amp;targetAccountId=2&amp;amount=500）。
/// </summary>
public class SendMoneySystemTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SendMoneySystemTest(WebApplicationFactory<Program> factory)
    {
        // 使用獨立的 in-memory 資料庫；初始測試資料由 Program.cs 啟動時自動植入，
        // 對應 Java 的 @Sql("SendMoneySystemTest.sql")。
        _factory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                // 移除正式 DbContext，改用隔離的 in-memory 資料庫
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BuckpalDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                // GUID 在 ConfigureServices 執行時生成一次，避免每個 scope 產生不同 DB 名稱
                var dbName = Guid.NewGuid().ToString();
                services.AddDbContext<BuckpalDbContext>(options =>
                    options.UseInMemoryDatabase(dbName));
            }));
    }

    /// <summary>
    /// 對應 Java：sendMoney()
    /// 驗證轉帳 500 後，來源帳戶餘額減少 500，目標帳戶餘額增加 500。
    /// </summary>
    [Fact]
    public async Task SendMoney_BalancesAreUpdatedCorrectly()
    {
        var client = _factory.CreateClient();

        var sourceAccountId  = 1L;
        var targetAccountId  = 2L;
        var transferredAmount = Money.Of(500m);

        var initialSourceBalance = GetBalance(sourceAccountId);
        var initialTargetBalance = GetBalance(targetAccountId);

        // Act：送出 HTTP POST 轉帳請求
        var response = await client.PostAsync(
            $"/SendMoney/send?sourceAccountId={sourceAccountId}&targetAccountId={targetAccountId}&amount={transferredAmount.Amount}",
            null);

        // Assert：HTTP 回應 200 OK
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert：來源帳戶餘額減少轉帳金額
        GetBalance(sourceAccountId).Should().Be(
            Money.Subtract(initialSourceBalance, transferredAmount),
            "來源帳戶應減少轉帳金額");

        // Assert：目標帳戶餘額增加轉帳金額
        GetBalance(targetAccountId).Should().Be(
            Money.Add(initialTargetBalance, transferredAmount),
            "目標帳戶應增加轉帳金額");
    }

    private Money GetBalance(long accountId)
    {
        using var scope = _factory.Services.CreateScope();
        var loadAccountPort = scope.ServiceProvider.GetRequiredService<ILoadAccountPort>();
        var account = loadAccountPort.LoadAccount(new AccountId(accountId), DateTime.Now);
        return account.CalculateBalance();
    }
}
