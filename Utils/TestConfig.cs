namespace PlaywrightPOM.Utils;

public static class TestConfig
{
    // 浏览器配置
    public static class Browser
    {
        public const string DefaultBrowserType = "chromium";
        public const bool DefaultHeadless = true;
        public const int DefaultTimeout = 30000;
        public const int DefaultViewportWidth = 1920;
        public const int DefaultViewportHeight = 1080;
    }

    // 测试URL配置
    public static class Urls
    {
        public const string SauceDemo = "https://www.saucedemo.com";
        public const string SauceDemoInventory = "https://www.saucedemo.com/inventory.html";
    }

    // 测试用户配置
    public static class Users
    {
        public const string StandardUser = "standard_user";
        public const string LockedOutUser = "locked_out_user";
        public const string ProblemUser = "problem_user";
        public const string PerformanceGlitchUser = "performance_glitch_user";
        public const string ErrorUser = "error_user";
        public const string VisualUser = "visual_user";
        public const string DefaultPassword = "secret_sauce";
    }

    // 错误消息配置
    public static class ErrorMessages
    {
        public const string LockedOutUserMessage = "Epic sadface: Sorry, this user has been locked out.";
        public const string InvalidCredentialsMessage = "Epic sadface: Username and password do not match any user in this service";
        public const string EmptyUsernameMessage = "Epic sadface: Username is required";
        public const string EmptyPasswordMessage = "Epic sadface: Password is required";
    }

    // 测试数据配置
    public static class TestData
    {
        public const string InvalidUsername = "invalid_user";
        public const string InvalidPassword = "invalid_password";
        public const string EmptyString = "";
    }

    // 等待时间配置
    public static class Timeouts
    {
        public const int ShortWait = 5000;
        public const int MediumWait = 10000;
        public const int LongWait = 30000;
        public const int ElementWait = 15000;
    }

    // 文件路径配置
    public static class Paths
    {
        public const string Screenshots = "Screenshots";
        public const string TestData = "TestData";
        public const string Reports = "Reports";
    }
}