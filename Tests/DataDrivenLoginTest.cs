using PlaywrightPOM.Pages;
using PlaywrightPOM.Utils;
using AventStack.ExtentReports;
using Xunit;

namespace PlaywrightPOM.Tests;

/// <summary>
/// 数据驱动登录测试类
/// </summary>
[Collection("LoginTests")]
public class DataDrivenLoginTest : BaseTest, IDisposable
{
    private LoginPage _loginPage = null!;
    private ProductsPage _productsPage = null!;
    private static int _testCount = 0;
    private static int _completedTests = 0;
    private static readonly object _lock = new object();
    
    public DataDrivenLoginTest()
    {
        lock (_lock)
        {
            _testCount++;
        }
    }

    /// <summary>
    /// 测试有效用户登录 - 数据驱动
    /// </summary>
    /// <param name="userData">用户数据</param>
    [Theory]
    [MemberData(nameof(GetValidUserData))]
    public async Task TestValidUserLogin(UserData userData)
    {
        // 初始化测试
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, $"TestValidUserLogin_{userData.Username}");
        
        // 开始测试报告
        StartTest($"有效用户登录测试 - {userData.Username}", userData.Description);
        
        try
        {
            // 初始化页面对象
            _loginPage = new LoginPage(Driver);
            _productsPage = new ProductsPage(Driver);
            
            // 导航到登录页面
            await Driver.Page.GotoAsync(TestConfig.Urls.SauceDemo);
            ExtentReportManager.LogStep(ExtentTest!, Status.Info, $"导航到登录页面: {TestConfig.Urls.SauceDemo}");
            
            // 执行登录
            await _loginPage.LoginAsync(userData.Username, userData.Password);
            ExtentReportManager.LogStep(ExtentTest!, Status.Info, $"使用用户 '{userData.Username}' 执行登录");
            
            // 验证登录成功
            var currentUrl = Driver.Page.Url;
            Assert.Contains("inventory", currentUrl);
            ExtentReportManager.LogStep(ExtentTest!, Status.Pass, $"登录成功，当前URL: {currentUrl}");
            
            // 验证页面标题
            var pageTitle = await Driver.Page.TitleAsync();
            Assert.Equal("Swag Labs", pageTitle);
            ExtentReportManager.LogStep(ExtentTest!, Status.Pass, $"页面标题验证成功: {pageTitle}");
            
            // 验证产品页面元素
            var productsTitle = await _productsPage.GetPageTitleAsync();
            Assert.Equal("Products", productsTitle);
            ExtentReportManager.LogStep(ExtentTest!, Status.Pass, "产品页面显示正常");
            
            // 截图
            await SaveScreenshotAsync($"ValidLogin_{userData.Username}");
            
            // 测试通过
            await CleanupAsync(true);
        }
        catch (Exception ex)
        {
            ExtentReportManager.LogStep(ExtentTest!, Status.Fail, $"测试失败: {ex.Message}");
            await SaveScreenshotAsync($"ValidLogin_{userData.Username}_Failed");
            await CleanupAsync(false, ex);
            throw;
        }
        finally
        {
            lock (_lock)
            {
                _completedTests++;
                if (_completedTests == _testCount)
                {
                    // 生成报告
                    ExtentReportManager.FlushReports();
                }
            }
        }
    }
    
    /// <summary>
    /// 测试无效用户登录 - 数据驱动
    /// </summary>
    /// <param name="userData">用户数据</param>
    [Theory]
    [MemberData(nameof(GetInvalidUserData))]
    public async Task TestInvalidUserLogin(UserData userData)
    {
        // 初始化测试
        await InitializeAsync(TestConfig.Browser.DefaultBrowserType, true, $"TestInvalidUserLogin_{userData.Username}");
        
        // 开始测试报告
        StartTest($"无效用户登录测试 - {userData.Username}", userData.Description);
        
        try
        {
            // 初始化页面对象
            _loginPage = new LoginPage(Driver);
            
            // 导航到登录页面
            await Driver.Page.GotoAsync(TestConfig.Urls.SauceDemo);
            ExtentReportManager.LogStep(ExtentTest!, Status.Info, $"导航到登录页面: {TestConfig.Urls.SauceDemo}");
            
            // 执行登录
            await _loginPage.LoginAsync(userData.Username, userData.Password);
            ExtentReportManager.LogStep(ExtentTest!, Status.Info, $"使用无效用户 '{userData.Username}' 执行登录");
            
            // 验证错误消息
            if (!string.IsNullOrEmpty(userData.ExpectedMessage))
            {
                var errorMessage = await _loginPage.GetErrorMessageAsync();
                Assert.Equal(userData.ExpectedMessage, errorMessage);
                ExtentReportManager.LogStep(ExtentTest!, Status.Pass, $"错误消息验证成功: {errorMessage}");
            }
            
            // 验证仍在登录页面
            var currentUrl = Driver.Page.Url;
            Assert.DoesNotContain("inventory", currentUrl);
            ExtentReportManager.LogStep(ExtentTest!, Status.Pass, $"验证仍在登录页面: {currentUrl}");
            
            // 截图
            await SaveScreenshotAsync($"InvalidLogin_{userData.Username}");
            
            // 测试通过
            await CleanupAsync(true);
        }
        catch (Exception ex)
        {
            ExtentReportManager.LogStep(ExtentTest!, Status.Fail, $"测试失败: {ex.Message}");
            await SaveScreenshotAsync($"InvalidLogin_{userData.Username}_Failed");
            await CleanupAsync(false, ex);
            throw;
        }
        finally
        {
            lock (_lock)
            {
                _completedTests++;
                if (_completedTests == _testCount)
                {
                    // 生成报告
                    ExtentReportManager.FlushReports();
                }
            }
        }
    }
    
    /// <summary>
    /// 获取有效用户测试数据
    /// </summary>
    /// <returns>有效用户数据集合</returns>
    public static IEnumerable<object[]> GetValidUserData()
    {
        var validUsers = TestDataManager.GetValidUsers();
        return validUsers.Select(user => new object[] { user });
    }
    
    /// <summary>
    /// 获取无效用户测试数据
    /// </summary>
    /// <returns>无效用户数据集合</returns>
    public static IEnumerable<object[]> GetInvalidUserData()
    {
        var invalidUsers = TestDataManager.GetInvalidUsers();
        return invalidUsers.Select(user => new object[] { user });
    }
    
    /// <summary>
    /// 资源清理
    /// </summary>
    public override void Dispose()
    {
        Driver?.Dispose();
    }
}