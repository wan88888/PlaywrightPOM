namespace PlaywrightPOM.Utils;

/// <summary>
/// 测试配置类，使用配置管理器获取配置
/// </summary>
public static class TestConfig
{
    private static BrowserConfig? _browserConfig;
    private static UrlConfig? _urlConfig;
    private static UserConfig? _userConfig;
    private static TimeoutConfig? _timeoutConfig;
    private static PathConfig? _pathConfig;

    /// <summary>
    /// 浏览器配置
    /// </summary>
    public static class Browser
    {
        private static BrowserConfig Config => _browserConfig ??= AppConfigurationManager.GetBrowserConfig();
        
        public static string DefaultBrowserType => Config.DefaultType;
        public static bool DefaultHeadless => Config.Headless;
        public static int DefaultTimeout => Config.Timeout;
        public static int DefaultViewportWidth => Config.ViewportWidth;
        public static int DefaultViewportHeight => Config.ViewportHeight;
    }

    /// <summary>
    /// URL配置
    /// </summary>
    public static class Urls
    {
        private static UrlConfig Config => _urlConfig ??= AppConfigurationManager.GetUrlConfig();
        
        public static string SauceDemo => Config.SauceDemo;
        public static string SauceDemoInventory => Config.SauceDemoInventory;
    }

    /// <summary>
    /// 用户配置
    /// </summary>
    public static class Users
    {
        private static UserConfig Config => _userConfig ??= AppConfigurationManager.GetUserConfig();
        
        public static string StandardUser => Config.StandardUser;
        public static string LockedOutUser => Config.LockedOutUser;
        public static string ProblemUser => Config.ProblemUser;
        public static string PerformanceGlitchUser => Config.PerformanceGlitchUser;
        public static string ErrorUser => Config.ErrorUser;
        public static string VisualUser => Config.VisualUser;
        public static string DefaultPassword => Config.DefaultPassword;
    }

    /// <summary>
    /// 错误消息配置
    /// </summary>
    public static class ErrorMessages
    {
        public static string LockedOutUserMessage => AppConfigurationManager.Configuration["ErrorMessages:LockedOutUser"] ?? "Epic sadface: Sorry, this user has been locked out.";
        public static string InvalidCredentialsMessage => AppConfigurationManager.Configuration["ErrorMessages:InvalidCredentials"] ?? "Epic sadface: Username and password do not match any user in this service";
        public static string EmptyUsernameMessage => AppConfigurationManager.Configuration["ErrorMessages:EmptyUsername"] ?? "Epic sadface: Username is required";
        public static string EmptyPasswordMessage => AppConfigurationManager.Configuration["ErrorMessages:EmptyPassword"] ?? "Epic sadface: Password is required";
    }

    /// <summary>
    /// 测试数据配置
    /// </summary>
    public static class TestData
    {
        public static string InvalidUsername => AppConfigurationManager.Configuration["TestData:InvalidUsername"] ?? "invalid_user";
        public static string InvalidPassword => AppConfigurationManager.Configuration["TestData:InvalidPassword"] ?? "invalid_password";
        public static string EmptyString => AppConfigurationManager.Configuration["TestData:EmptyString"] ?? "";
    }

    /// <summary>
    /// 超时配置
    /// </summary>
    public static class Timeouts
    {
        private static TimeoutConfig Config => _timeoutConfig ??= AppConfigurationManager.GetTimeoutConfig();
        
        public static int ShortWait => Config.ShortWait;
        public static int MediumWait => Config.MediumWait;
        public static int LongWait => Config.LongWait;
        public static int ElementWait => Config.ElementWait;
    }

    /// <summary>
    /// 路径配置
    /// </summary>
    public static class Paths
    {
        private static PathConfig Config => _pathConfig ??= AppConfigurationManager.GetPathConfig();
        
        public static string Screenshots => Config.Screenshots;
        public static string TestData => Config.TestData;
        public static string Reports => Config.Reports;
    }
}