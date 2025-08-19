using PlaywrightPOM.Utils;

namespace PlaywrightPOM.Pages;

public class LoginPage : BasePage
{
    // 页面元素选择器
    private const string UsernameInput = "[data-test='username']";
    private const string PasswordInput = "[data-test='password']";
    private const string LoginButton = "[data-test='login-button']";
    private const string ErrorMessage = "[data-test='error']";
    private const string LoginLogo = ".login_logo";
    private const string LoginContainer = "#login_button_container";

    // 页面URL
    public const string LoginUrl = "https://www.saucedemo.com";

    public LoginPage(IPlaywrightDriver driver) : base(driver)
    {
    }

    /// <summary>
    /// 导航到登录页面
    /// </summary>
    public async Task NavigateToLoginPageAsync()
    {
        await NavigateToAsync(LoginUrl);
        await WaitForLoadStateAsync();
    }

    /// <summary>
    /// 输入用户名
    /// </summary>
    /// <param name="username">用户名</param>
    public async Task EnterUsernameAsync(string username)
    {
        await WaitForElementAsync(UsernameInput);
        await FillAsync(UsernameInput, username);
    }

    /// <summary>
    /// 输入密码
    /// </summary>
    /// <param name="password">密码</param>
    public async Task EnterPasswordAsync(string password)
    {
        await WaitForElementAsync(PasswordInput);
        await FillAsync(PasswordInput, password);
    }

    /// <summary>
    /// 点击登录按钮
    /// </summary>
    public async Task ClickLoginButtonAsync()
    {
        await ClickAsync(LoginButton);
    }

    /// <summary>
    /// 执行登录操作
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    public async Task LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginButtonAsync();
    }

    /// <summary>
    /// 获取错误消息
    /// </summary>
    /// <returns>错误消息文本</returns>
    public async Task<string> GetErrorMessageAsync()
    {
        try
        {
            await WaitForElementAsync(ErrorMessage, 5000);
            return await GetTextAsync(ErrorMessage);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 检查是否显示错误消息
    /// </summary>
    /// <returns>是否显示错误消息</returns>
    public async Task<bool> IsErrorMessageDisplayedAsync()
    {
        return await IsVisibleAsync(ErrorMessage);
    }

    /// <summary>
    /// 检查登录页面是否加载完成
    /// </summary>
    /// <returns>登录页面是否加载完成</returns>
    public async Task<bool> IsLoginPageLoadedAsync()
    {
        return await IsVisibleAsync(LoginLogo) && 
               await IsVisibleAsync(LoginContainer) && 
               await IsVisibleAsync(UsernameInput) && 
               await IsVisibleAsync(PasswordInput) && 
               await IsVisibleAsync(LoginButton);
    }

    /// <summary>
    /// 清空用户名输入框
    /// </summary>
    public async Task ClearUsernameAsync()
    {
        await Page.FillAsync(UsernameInput, "");
    }

    /// <summary>
    /// 清空密码输入框
    /// </summary>
    public async Task ClearPasswordAsync()
    {
        await Page.FillAsync(PasswordInput, "");
    }

    /// <summary>
    /// 清空所有输入框
    /// </summary>
    public async Task ClearAllInputsAsync()
    {
        await ClearUsernameAsync();
        await ClearPasswordAsync();
    }
}