using NetArchTest.Rules;
using FluentAssertions;
using Xunit;

namespace Buckpal_CSharp_Test;

/// <summary>
/// 六角形架構依賴規則測試（對應 Java DependencyRuleTests + archunit/ 下的輔助類別）。
///
/// 對應 Java ArchUnit 規則：
///   - Domain 層不可依賴 Adapters 層或 Application 層
///   - Application 層不可依賴 Adapters 層
///   - Adapters.In（傳入轉接器）不可依賴 Adapters.Out（傳出轉接器），反之亦然
///   - Application Ports.In 不可依賴 Application Ports.Out，反之亦然
///
/// C# 使用 NetArchTest.Rules 取代 Java 的 ArchUnit。
/// </summary>
public class DependencyRuleTests
{
    private const string DomainModelNamespace = "Buckpal_CSharp.Domain.Model";
    private const string AppNamespace          = "Buckpal_CSharp.Application";
    private const string AdaptersNamespace     = "Buckpal_CSharp.Adapters";
    private const string AdaptersIn            = "Buckpal_CSharp.Adapters.In";
    private const string AdaptersOut           = "Buckpal_CSharp.Adapters.Out";
    private const string PortsIn               = "Buckpal_CSharp.Application.Ports.In";
    private const string PortsOut              = "Buckpal_CSharp.Application.Ports.Out";

    /// <summary>
    /// 對應 Java validateRegistrationContextArchitecture() 中的 domainDoesNotDependOnOtherPackages：
    /// Domain.Model（純領域實體）不可依賴 Adapters 層。
    /// 注意：C# 的 Domain.Service 屬應用服務，允許依賴 Application.Ports，
    ///       對應 Java 的 application.service 套件，故只驗證 Domain.Model。
    /// </summary>
    [Fact]
    public void DomainLayer_ShouldNotDependOn_AdaptersLayer()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(DomainModelNamespace)
            .ShouldNot().HaveDependencyOn(AdaptersNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Domain.Model 不可依賴 Adapters 層，違反六角形架構原則。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>
    /// Domain.Model（純領域實體）不可依賴 Application 層（Port / Service）。
    /// </summary>
    [Fact]
    public void DomainLayer_ShouldNotDependOn_ApplicationLayer()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(DomainModelNamespace)
            .ShouldNot().HaveDependencyOn(AppNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Domain.Model 不可依賴 Application 層。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>
    /// 對應 Java：applicationLayer.doesNotDependOn(adapters.getBasePackage())
    /// Application 層（含 Ports 與 Service）不可依賴 Adapters 層。
    /// </summary>
    [Fact]
    public void ApplicationLayer_ShouldNotDependOn_AdaptersLayer()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(AppNamespace)
            .ShouldNot().HaveDependencyOn(AdaptersNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Application 層不可依賴 Adapters 層。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>
    /// 對應 Java：adapters.dontDependOnEachOther()
    /// 傳入轉接器（Adapters.In）不可依賴傳出轉接器（Adapters.Out）。
    /// </summary>
    [Fact]
    public void IncomingAdapters_ShouldNotDependOn_OutgoingAdapters()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(AdaptersIn)
            .ShouldNot().HaveDependencyOn(AdaptersOut)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "傳入轉接器（Adapters.In）不可依賴傳出轉接器（Adapters.Out）。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>
    /// 傳出轉接器（Adapters.Out）不可依賴傳入轉接器（Adapters.In）。
    /// </summary>
    [Fact]
    public void OutgoingAdapters_ShouldNotDependOn_IncomingAdapters()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(AdaptersOut)
            .ShouldNot().HaveDependencyOn(AdaptersIn)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "傳出轉接器（Adapters.Out）不可依賴傳入轉接器（Adapters.In）。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>
    /// 對應 Java：applicationLayer.incomingAndOutgoingPortsDoNotDependOnEachOther()
    /// 輸入 Port（Ports.In）不可依賴輸出 Port（Ports.Out）。
    /// </summary>
    [Fact]
    public void IncomingPorts_ShouldNotDependOn_OutgoingPorts()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(PortsIn)
            .ShouldNot().HaveDependencyOn(PortsOut)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Ports.In 不可依賴 Ports.Out。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>
    /// 輸出 Port（Ports.Out）不可依賴輸入 Port（Ports.In）。
    /// </summary>
    [Fact]
    public void OutgoingPorts_ShouldNotDependOn_IncomingPorts()
    {
        var result = Types.InAssembly(typeof(Buckpal_CSharp.Domain.Model.Account).Assembly)
            .That().ResideInNamespace(PortsOut)
            .ShouldNot().HaveDependencyOn(PortsIn)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Ports.Out 不可依賴 Ports.In。\n失敗類別：{0}",
            string.Join(", ", result.FailingTypeNames ?? []));
    }
}
