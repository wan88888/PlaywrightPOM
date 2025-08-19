using PlaywrightPOM.Pages;
using PlaywrightPOM.Utils;
using AventStack.ExtentReports;
using Xunit;

namespace PlaywrightPOM.Tests;

[Collection("LoginTests")]
public class LoginTest : BaseTest, IDisposable
{
    private LoginPage _loginPage = null!;
    private ProductsPage _productsPage = null!;
    private static int _testCount = 0;
    private static int _completedTests = 0;
    private static readonly object _lock = new object();
    
    public LoginTest()
    {
        lock (_lock)
        {
            _testCount++;
        }
    }

    /// <summary>
    /// 测试成功登录场景
    /// </summary>
    [Fact]
    public async Task TestSuccessfulLogin()
    {
        // 初始化测试（无头模式）
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, nameof(TestSuccessfulLogin));
        StartTest(nameof(TestSuccessfulLogin));
        LogStep(Status.Info, "验证标准用户能够成功登录系统");
        AssignCategory("登录测试", "正向测试");
        AssignAuthor("测试工程师");
        
        _loginPage = new LoginPage(Driver);
        _productsPage = new ProductsPage(Driver);

        try
        {
            // 导航到登录页面
            LogStep(Status.Info, "导航到登录页面");
            await _loginPage.NavigateToLoginPageAsync();
            
            // 验证登录页面加载完成
            LogStep(Status.Info, "验证登录页面是否正确加载");
            var isLoginPageLoaded = await _loginPage.IsLoginPageLoadedAsync();
            Assert.True(isLoginPageLoaded, "登录页面未正确加载");
            LogStep(Status.Pass, "登录页面加载成功");

            // 执行登录操作
            LogStep(Status.Info, $"使用用户名 '{TestConfig.Users.StandardUser}' 执行登录");
            await _loginPage.LoginAsync(TestConfig.Users.StandardUser, TestConfig.Users.DefaultPassword);

            // 等待页面跳转
            LogStep(Status.Info, "等待页面跳转");
            await Task.Delay(2000);

            // 验证登录成功
            LogStep(Status.Info, "验证是否成功跳转到产品页面");
            var isLoginSuccessful = await _productsPage.VerifySuccessfulLoginAsync();
            Assert.True(isLoginSuccessful, "登录失败，未能跳转到产品页面");
            LogStep(Status.Pass, "成功跳转到产品页面");

            // 验证页面标题
            LogStep(Status.Info, "验证页面标题");
            var pageTitle = await _productsPage.GetPageTitleAsync();
            Assert.Equal(ProductsPage.ExpectedTitle, pageTitle);
            LogStep(Status.Pass, $"页面标题验证成功: {pageTitle}");

            // 验证URL包含inventory.html
            LogStep(Status.Info, "验证页面URL");
            var currentUrl = _productsPage.GetCurrentUrl();
            Assert.Contains("inventory.html", currentUrl);
            LogStep(Status.Pass, $"URL验证成功: {currentUrl}");

            // 保存成功截图
            var screenshotPath = await SaveScreenshotAsync("successful_login");
            ExtentReportManager.AddScreenshot(ExtentTest!, screenshotPath, "登录成功截图");
            
            await CleanupAsync(true);
        }
        catch (Exception ex)
        {
            await CleanupAsync(false, ex);
            throw;
        }
    }

    /// <summary>
    /// 测试无效用户名登录场景
    /// </summary>
    [Fact]
    public async Task TestInvalidUsernameLogin()
    {
        // 初始化测试（无头模式）
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, nameof(TestInvalidUsernameLogin));
        StartTest(nameof(TestInvalidUsernameLogin));
        LogStep(Status.Info, "验证无效用户名无法登录系统");
        AssignCategory("登录测试", "负向测试");
        AssignAuthor("测试工程师");
        
        _loginPage = new LoginPage(Driver);

        try
        {
            // 导航到登录页面
            LogStep(Status.Info, "导航到登录页面");
            await _loginPage.NavigateToLoginPageAsync();
            
            // 验证登录页面加载完成
            LogStep(Status.Info, "验证登录页面是否正确加载");
            var isLoginPageLoaded = await _loginPage.IsLoginPageLoadedAsync();
            Assert.True(isLoginPageLoaded, "登录页面未正确加载");
            LogStep(Status.Pass, "登录页面加载成功");
            
            // 使用无效用户名登录
            LogStep(Status.Info, $"使用无效用户名 '{TestConfig.TestData.InvalidUsername}' 尝试登录");
            await _loginPage.LoginAsync(TestConfig.TestData.InvalidUsername, TestConfig.Users.DefaultPassword);
            
            // 等待错误消息显示
            LogStep(Status.Info, "等待错误消息显示");
            await Task.Delay(1000);
            
            // 验证错误消息显示
            LogStep(Status.Info, "验证是否显示错误消息");
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            Assert.True(isErrorDisplayed, "未显示错误消息");
            LogStep(Status.Pass, "错误消息显示成功");
            
            // 验证错误消息内容
            LogStep(Status.Info, "验证错误消息内容");
            var errorMessage = await _loginPage.GetErrorMessageAsync();
            Assert.Contains("Username and password do not match", errorMessage);
            LogStep(Status.Pass, $"错误消息验证成功: {errorMessage}");
            
            // 保存失败截图
            var screenshotPath = await SaveScreenshotAsync("invalid_username_login");
            ExtentReportManager.AddScreenshot(ExtentTest!, screenshotPath, "无效用户名登录截图");
            
            await CleanupAsync(true);
        }
        catch (Exception ex)
        {
            await CleanupAsync(false, ex);
            throw;
        }
    }
    
    public new void Dispose()
    {
        lock (_lock)
        {
            _completedTests++;
            if (_completedTests == _testCount)
            {
                // 所有测试完成后生成报告
                FlushReports();
                Console.WriteLine($"所有 {_testCount} 个测试已完成，测试报告已生成");
            }
        }
        base.Dispose();
    }
}