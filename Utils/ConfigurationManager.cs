using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace PlaywrightPOM.Utils;

/// <summary>
/// 配置管理器，用于管理应用程序配置
/// </summary>
public static class AppConfigurationManager
{
    private static IConfiguration? _configuration;
    private static readonly object _lock = new object();

    /// <summary>
    /// 获取配置实例
    /// </summary>
    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                lock (_lock)
                {
                    if (_configuration == null)
                    {
                        InitializeConfiguration();
                    }
                }
            }
            return _configuration!;
        }
    }

    /// <summary>
    /// 公共初始化方法
    /// </summary>
    public static void Initialize()
    {
        // 触发配置初始化
        _ = Configuration;
    }

    /// <summary>
    /// 初始化配置
    /// </summary>
    private static void InitializeConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Development";
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory();

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("TEST_");

        _configuration = builder.Build();
        
        Console.WriteLine($"配置已加载，当前环境: {environment}");
    }

    /// <summary>
    /// 获取浏览器配置
    /// </summary>
    public static BrowserConfig GetBrowserConfig()
    {
        return new BrowserConfig
        {
            DefaultType = Configuration["Browser:DefaultType"] ?? "chromium",
            Headless = bool.Parse(Configuration["Browser:Headless"] ?? "true"),
            Timeout = int.Parse(Configuration["Browser:Timeout"] ?? "30000"),
            ViewportWidth = int.Parse(Configuration["Browser:ViewportWidth"] ?? "1920"),
            ViewportHeight = int.Parse(Configuration["Browser:ViewportHeight"] ?? "1080")
        };
    }

    /// <summary>
    /// 获取URL配置
    /// </summary>
    public static UrlConfig GetUrlConfig()
    {
        return new UrlConfig
        {
            SauceDemo = Configuration["Urls:SauceDemo"] ?? "https://www.saucedemo.com",
            SauceDemoInventory = Configuration["Urls:SauceDemoInventory"] ?? "https://www.saucedemo.com/inventory.html"
        };
    }

    /// <summary>
    /// 获取用户配置
    /// </summary>
    public static UserConfig GetUserConfig()
    {
        return new UserConfig
        {
            StandardUser = Configuration["Users:StandardUser"] ?? "standard_user",
            LockedOutUser = Configuration["Users:LockedOutUser"] ?? "locked_out_user",
            ProblemUser = Configuration["Users:ProblemUser"] ?? "problem_user",
            PerformanceGlitchUser = Configuration["Users:PerformanceGlitchUser"] ?? "performance_glitch_user",
            ErrorUser = Configuration["Users:ErrorUser"] ?? "error_user",
            VisualUser = Configuration["Users:VisualUser"] ?? "visual_user",
            DefaultPassword = Configuration["Users:DefaultPassword"] ?? "secret_sauce"
        };
    }

    /// <summary>
    /// 获取超时配置
    /// </summary>
    public static TimeoutConfig GetTimeoutConfig()
    {
        return new TimeoutConfig
        {
            ShortWait = int.Parse(Configuration["Timeouts:ShortWait"] ?? "5000"),
            MediumWait = int.Parse(Configuration["Timeouts:MediumWait"] ?? "10000"),
            LongWait = int.Parse(Configuration["Timeouts:LongWait"] ?? "30000"),
            ElementWait = int.Parse(Configuration["Timeouts:ElementWait"] ?? "15000")
        };
    }

    /// <summary>
    /// 获取路径配置
    /// </summary>
    public static PathConfig GetPathConfig()
    {
        return new PathConfig
        {
            Screenshots = Configuration["Paths:Screenshots"] ?? "Screenshots",
            TestData = Configuration["Paths:TestData"] ?? "TestData",
            Reports = Configuration["Paths:Reports"] ?? "ExtentReports"
        };
    }
}

/// <summary>
/// 浏览器配置
/// </summary>
public class BrowserConfig
{
    public string DefaultType { get; set; } = "chromium";
    public bool Headless { get; set; } = true;
    public int Timeout { get; set; } = 30000;
    public int ViewportWidth { get; set; } = 1920;
    public int ViewportHeight { get; set; } = 1080;
}

/// <summary>
/// URL配置
/// </summary>
public class UrlConfig
{
    public string SauceDemo { get; set; } = "https://www.saucedemo.com";
    public string SauceDemoInventory { get; set; } = "https://www.saucedemo.com/inventory.html";
}

/// <summary>
/// 用户配置
/// </summary>
public class UserConfig
{
    public string StandardUser { get; set; } = "standard_user";
    public string LockedOutUser { get; set; } = "locked_out_user";
    public string ProblemUser { get; set; } = "problem_user";
    public string PerformanceGlitchUser { get; set; } = "performance_glitch_user";
    public string ErrorUser { get; set; } = "error_user";
    public string VisualUser { get; set; } = "visual_user";
    public string DefaultPassword { get; set; } = "secret_sauce";
}

/// <summary>
/// 超时配置
/// </summary>
public class TimeoutConfig
{
    public int ShortWait { get; set; } = 5000;
    public int MediumWait { get; set; } = 10000;
    public int LongWait { get; set; } = 30000;
    public int ElementWait { get; set; } = 15000;
}

/// <summary>
/// 路径配置
/// </summary>
public class PathConfig
{
    public string Screenshots { get; set; } = "Screenshots";
    public string TestData { get; set; } = "TestData";
    public string Reports { get; set; } = "ExtentReports";
}