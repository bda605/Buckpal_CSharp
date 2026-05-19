using System.Net;
using Buckpal_CSharp.Adapters.Out.Persistence;
using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Domain.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Buckpal_CSharp_Test.Adapter.In.Web;

/// <summary>
/// SendMoneyController Web 轉接器整合測試（對應 Java SendMoneyControllerTest）。
///
/// 與 Java 版本的差異說明：
///   - Java 使用 Spring @WebMvcTest + MockMvc，只載入 Web 層的 Spring Context。
///   - C# 使用 WebApplicationFactory&lt;Program&gt; 啟動完整 ASP.NET Core 測試主機，
///     並透過 ConfigureServices 將 ISendMoneyUseCase 替換為 NSubstitute Substitute 物件。
///
///   - Java 路由（Path Variable）：POST /accounts/send/41/42/500
///   - C# 路由（Query String）  ：POST /SendMoney/send?sourceAccountId=41&amp;targetAccountId=42&amp;amount=500
///
/// 測試類別實作 IClassFixture&lt;WebApplicationFactory&lt;Program&gt;&gt; 以共用測試主機，
/// 減少每個測試方法重複啟動的成本。
/// </summary>
public class SendMoneyControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ISendMoneyUseCase _mockUseCase = Substitute.For<ISendMoneyUseCase>();
    private readonly HttpClient        _client;

    public SendMoneyControllerTest(WebApplicationFactory<Program> factory)
    {
        // 預設讓 Substitute 的 SendMoney 回傳 true（轉帳成功）
        _mockUseCase.SendMoney(Arg.Any<SendMoneyCommand>()).Returns(true);

        // 建立測試用 HttpClient，並以 Mock 替換正式 UseCase 實作
        _client = factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    // 移除正式 DbContext 註冊，改用獨立的 in-memory 資料庫（避免多個測試 host 共用同名 DB 造成重複 key 錯誤）
                    var dbDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<BuckpalDbContext>));
                    if (dbDescriptor is not null)
                        services.Remove(dbDescriptor);
                    var dbName = Guid.NewGuid().ToString();
                    services.AddDbContext<BuckpalDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));

                    // 移除 DI 容器中的正式 ISendMoneyUseCase 實作
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ISendMoneyUseCase));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    // 注入 NSubstitute Substitute 物件取代正式實作
                    services.AddSingleton(_mockUseCase);
                }))
            .CreateClient();
    }

    /// <summary>
    /// 【匯款 HTTP 請求】
    /// POST /SendMoney/send?sourceAccountId=41&amp;targetAccountId=42&amp;amount=500
    /// 應回傳 HTTP 200 OK，且 UseCase 應收到正確組裝的 SendMoneyCommand。
    ///
    /// 對應 Java：testSendMoney()
    /// （Java 驗證 POST /accounts/send/41/42/500 path variable；
    ///   C# 驗證 query string 格式的相同語意）
    /// </summary>
    [Fact]
    public async Task TestSendMoney_ReturnsOk_AndCommandIsCorrect()
    {
        // Arrange：組裝預期的 Command（值相等比較，因 SendMoneyCommand 為 record）
        var expectedCommand = new SendMoneyCommand(
            new AccountId(41L),
            new AccountId(42L),
            Money.Of(500m));

        // Act：發送 HTTP POST 請求（使用 query string 格式）
        var response = await _client.PostAsync(
            "/SendMoney/send?sourceAccountId=41&targetAccountId=42&amount=500",
            content: null);

        // Assert — HTTP 狀態碼應為 200 OK
        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "轉帳成功時 Controller 應回傳 200 OK");

        // Assert — UseCase 應被呼叫一次，且傳入的 Command 與預期值相等
        _mockUseCase.Received(1).SendMoney(expectedCommand);
    }

    /// <summary>
    /// 【轉帳失敗回傳 BadRequest】
    /// 當 UseCase.SendMoney 回傳 false 時，
    /// Controller 應回傳 HTTP 400 Bad Request。
    /// </summary>
    [Fact]
    public async Task TestSendMoney_WhenUseCaseFails_ReturnsBadRequest()
    {
        // Arrange：讓 Substitute 回傳 false（轉帳失敗）
        _mockUseCase.SendMoney(Arg.Any<SendMoneyCommand>()).Returns(false);

        // Act
        var response = await _client.PostAsync(
            "/SendMoney/send?sourceAccountId=41&targetAccountId=42&amount=500",
            content: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
            "轉帳失敗時 Controller 應回傳 400 Bad Request");
    }
}
