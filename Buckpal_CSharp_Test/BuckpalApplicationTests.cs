using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Buckpal_CSharp_Test;

/// <summary>
/// 應用程式啟動測試（對應 Java BuckPalApplicationTests）。
/// 驗證整個 ASP.NET Core DI 容器與中介軟體管線能正常建立，不拋出任何例外。
/// </summary>
public class BuckpalApplicationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BuckpalApplicationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// 對應 Java：contextLoads()
    /// 確認應用程式能夠正常啟動，DI 容器設定無誤。
    /// </summary>
    [Fact]
    public void ApplicationContextLoads()
    {
        // 建立 HttpClient 即代表應用程式成功啟動，若啟動失敗會拋出例外使測試失敗
        var client = _factory.CreateClient();
        Assert.NotNull(client);
    }
}
