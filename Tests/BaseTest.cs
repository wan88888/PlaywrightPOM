using Microsoft.Playwright;
using PlaywrightPOM.Utils;
using AventStack.ExtentReports;

namespace PlaywrightPOM.Tests;

public abstract class BaseTest : IDisposable
{
    protected IPlaywrightDriver Driver { get; private set; } = null!;
    protected string TestName { get; private set; } = string.Empty;
    protected ExtentTest? ExtentTest { get; private set; }

    protected BaseTest()
    {
        // 初始化配置管理器
        AppConfigurationManager.Initialize();
    }

    /// <summary>
    /// 测试初始化
    /// </summary>
    /// <param name="browserType">浏览器类型（chromium, firefox, webkit）</param>
    /// <param name="headless">是否无头模式</param>
    /// <param name="testName">测试名称</param>
    protected async Task InitializeAsync(string browserType = "chromium", bool headless = true, string testName = "")
    {
        TestName = testName;
        Driver = new PlaywrightDriver();
        await Driver.InitializeAsync(browserType, headless);
    }

    /// <summary>
    /// 开始测试并创建ExtentTest实例
    /// </summary>
    /// <param name="testName">测试名称</param>
    /// <param name="description">测试描述</param>
    protected void StartTest(string testName, string description = "")
    {
        ExtentTest = ExtentReportManager.CreateTest(testName, description);
        ExtentReportManager.LogStep(ExtentTest, Status.Info, $"开始执行测试: {testName}");
    }

    /// <summary>
    /// 测试清理
    /// </summary>
    protected async Task CleanupAsync(bool testPassed = true, Exception? exception = null)
    {
        if (ExtentTest != null)
        {
            if (testPassed)
            {
                ExtentReportManager.LogStep(ExtentTest, Status.Pass, "测试执行成功");
            }
            else
            {
                ExtentReportManager.LogStep(ExtentTest, Status.Fail, "测试执行失败");
                
                if (exception != null)
                {
                    ExtentReportManager.LogException(ExtentTest, exception);
                }

                if (Driver != null)
                {
                    var screenshotPath = await SaveScreenshotAsync(TestName);
                    ExtentReportManager.AddScreenshot(ExtentTest, screenshotPath, "失败截图");
                }
            }
        }

        if (Driver != null)
        {
            await Driver.CloseAsync();
        }
    }

    /// <summary>
    /// 获取测试数据目录路径
    /// </summary>
    /// <returns>测试数据目录路径</returns>
    protected string GetTestDataPath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
    }

    /// <summary>
    /// 获取截图目录路径
    /// </summary>
    /// <returns>截图目录路径</returns>
    protected string GetScreenshotPath()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
        Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// 保存测试截图
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>截图文件路径</returns>
    protected async Task<string> SaveScreenshotAsync(string fileName)
    {
        var screenshotPath = Path.Combine(GetScreenshotPath(), $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        await Driver.Page.ScreenshotAsync(new Microsoft.Playwright.PageScreenshotOptions { Path = screenshotPath });
        return screenshotPath;
    }

    /// <summary>
    /// 等待指定时间
    /// </summary>
    /// <param name="milliseconds">等待时间（毫秒）</param>
    protected async Task WaitAsync(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    /// <summary>
    /// 记录测试步骤
    /// </summary>
    /// <param name="status">状态</param>
    /// <param name="details">详细信息</param>
    protected void LogStep(Status status, string details)
    {
        if (ExtentTest != null)
        {
            ExtentReportManager.LogStep(ExtentTest, status, details);
        }
    }
    
    /// <summary>
    /// 添加测试分类
    /// </summary>
    /// <param name="categories">分类标签</param>
    protected void AssignCategory(params string[] categories)
    {
        if (ExtentTest != null)
        {
            ExtentReportManager.AssignCategory(ExtentTest, categories);
        }
    }
    
    /// <summary>
    /// 设置测试作者
    /// </summary>
    /// <param name="authors">作者</param>
    protected void AssignAuthor(params string[] authors)
    {
        if (ExtentTest != null)
        {
            ExtentReportManager.AssignAuthor(ExtentTest, authors);
        }
    }
    
    /// <summary>
    /// 生成并刷新测试报告
    /// </summary>
    public static void FlushReports()
    {
        ExtentReportManager.FlushReports();
        Console.WriteLine("测试报告已生成完成");
    }
    
    /// <summary>
    /// 打开测试报告
    /// </summary>
    public static void OpenReport()
    {
        ExtentReportManager.OpenReport();
    }

    public virtual void Dispose()
    {
        CleanupAsync().GetAwaiter().GetResult();
    }
}