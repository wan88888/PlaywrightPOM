using PlaywrightPOM.Pages;
using PlaywrightPOM.Utils;
using AventStack.ExtentReports;

namespace PlaywrightPOM.Tests;

public class LoginTest : BaseTest
{
    private LoginPage _loginPage = null!;
    private ProductsPage _productsPage = null!;

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
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, nameof(TestInvalidUsernameLogin));
        
        _loginPage = new LoginPage(Driver);

        try
        {
            await _loginPage.NavigateToLoginPageAsync();
            
            // 使用无效用户名登录
            await _loginPage.LoginAsync(TestConfig.TestData.InvalidUsername, TestConfig.Users.DefaultPassword);
            
            // 等待错误消息显示
            await Task.Delay(1000);
            
            // 验证错误消息显示
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            Assert.True(isErrorDisplayed, "未显示错误消息");
            
            var errorMessage = await _loginPage.GetErrorMessageAsync();
            Assert.Contains("Username and password do not match", errorMessage);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    /// <summary>
    /// 测试锁定用户登录场景
    /// </summary>
    [Fact]
    public async Task TestLockedOutUserLogin()
    {
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, nameof(TestLockedOutUserLogin));
        
        _loginPage = new LoginPage(Driver);

        try
        {
            await _loginPage.NavigateToLoginPageAsync();
            
            // 使用锁定用户登录
            await _loginPage.LoginAsync(TestConfig.Users.LockedOutUser, TestConfig.Users.DefaultPassword);
            
            // 等待错误消息显示
            await Task.Delay(1000);
            
            // 验证错误消息显示
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            Assert.True(isErrorDisplayed, "未显示错误消息");
            
            var errorMessage = await _loginPage.GetErrorMessageAsync();
            Assert.Contains("locked out", errorMessage);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    /// <summary>
    /// 测试空用户名登录场景
    /// </summary>
    [Fact]
    public async Task TestEmptyUsernameLogin()
    {
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, nameof(TestEmptyUsernameLogin));
        
        _loginPage = new LoginPage(Driver);

        try
        {
            await _loginPage.NavigateToLoginPageAsync();
            
            // 使用空用户名登录
            await _loginPage.LoginAsync(TestConfig.TestData.EmptyString, TestConfig.Users.DefaultPassword);
            
            // 等待错误消息显示
            await Task.Delay(1000);
            
            // 验证错误消息显示
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            Assert.True(isErrorDisplayed, "未显示错误消息");
            
            var errorMessage = await _loginPage.GetErrorMessageAsync();
            Assert.Contains("Username is required", errorMessage);
        }
        finally
        {
            await CleanupAsync();
        }
    }

    /// <summary>
    /// 测试空密码登录场景
    /// </summary>
    [Fact]
    public async Task TestEmptyPasswordLogin()
    {
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, nameof(TestEmptyPasswordLogin));
        
        _loginPage = new LoginPage(Driver);

        try
        {
            await _loginPage.NavigateToLoginPageAsync();
            
            // 使用空密码登录
            await _loginPage.LoginAsync(TestConfig.Users.StandardUser, TestConfig.TestData.EmptyString);
            
            // 等待错误消息显示
            await Task.Delay(1000);
            
            // 验证错误消息显示
            var isErrorDisplayed = await _loginPage.IsErrorMessageDisplayedAsync();
            Assert.True(isErrorDisplayed, "未显示错误消息");
            
            var errorMessage = await _loginPage.GetErrorMessageAsync();
            Assert.Contains("Password is required", errorMessage);
        }
        finally
        {
            await CleanupAsync();
        }
    }
}