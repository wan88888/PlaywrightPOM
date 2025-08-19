using Microsoft.Playwright;
using PlaywrightPOM.Utils;

namespace PlaywrightPOM.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly IPlaywrightDriver Driver;

    protected BasePage(IPlaywrightDriver driver)
    {
        Driver = driver;
        Page = driver.Page;
    }

    /// <summary>
    /// 导航到指定URL
    /// </summary>
    /// <param name="url">目标URL</param>
    public virtual async Task NavigateToAsync(string url)
    {
        await Page.GotoAsync(url);
    }

    /// <summary>
    /// 等待页面加载完成
    /// </summary>
    public virtual async Task WaitForLoadStateAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// 点击元素
    /// </summary>
    /// <param name="selector">元素选择器</param>
    public virtual async Task ClickAsync(string selector)
    {
        await Page.ClickAsync(selector);
    }

    /// <summary>
    /// 输入文本
    /// </summary>
    /// <param name="selector">元素选择器</param>
    /// <param name="text">要输入的文本</param>
    public virtual async Task FillAsync(string selector, string text)
    {
        await Page.FillAsync(selector, text);
    }

    /// <summary>
    /// 获取元素文本
    /// </summary>
    /// <param name="selector">元素选择器</param>
    /// <returns>元素文本内容</returns>
    public virtual async Task<string> GetTextAsync(string selector)
    {
        return await Page.TextContentAsync(selector) ?? string.Empty;
    }

    /// <summary>
    /// 等待元素可见
    /// </summary>
    /// <param name="selector">元素选择器</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    public virtual async Task WaitForElementAsync(string selector, float timeout = 30000)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeout });
    }

    /// <summary>
    /// 检查元素是否可见
    /// </summary>
    /// <param name="selector">元素选择器</param>
    /// <returns>元素是否可见</returns>
    public virtual async Task<bool> IsVisibleAsync(string selector)
    {
        return await Page.IsVisibleAsync(selector);
    }

    /// <summary>
    /// 获取页面标题
    /// </summary>
    /// <returns>页面标题</returns>
    public virtual async Task<string> GetTitleAsync()
    {
        return await Page.TitleAsync();
    }

    /// <summary>
    /// 获取当前页面URL
    /// </summary>
    /// <returns>当前页面URL</returns>
    public virtual string GetCurrentUrl()
    {
        return Page.Url;
    }

    /// <summary>
    /// 截图
    /// </summary>
    /// <param name="path">截图保存路径</param>
    public virtual async Task TakeScreenshotAsync(string path)
    {
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
    }
}