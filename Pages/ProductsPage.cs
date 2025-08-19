using PlaywrightPOM.Utils;

namespace PlaywrightPOM.Pages;

public class ProductsPage : BasePage
{
    // 页面元素选择器
    private const string ProductsTitle = ".title";
    private const string ProductsContainer = ".inventory_container";
    private const string ShoppingCartLink = ".shopping_cart_link";
    private const string MenuButton = "#react-burger-menu-btn";
    private const string InventoryItems = ".inventory_item";
    private const string AppLogo = ".app_logo";

    // 预期的页面标题
    public const string ExpectedTitle = "Products";
    public const string ProductsUrl = "https://www.saucedemo.com/inventory.html";

    public ProductsPage(IPlaywrightDriver driver) : base(driver)
    {
    }

    /// <summary>
    /// 检查是否在产品页面
    /// </summary>
    /// <returns>是否在产品页面</returns>
    public async Task<bool> IsOnProductsPageAsync()
    {
        try
        {
            await WaitForElementAsync(ProductsTitle, 10000);
            var title = await GetTextAsync(ProductsTitle);
            return title.Equals(ExpectedTitle, StringComparison.OrdinalIgnoreCase) && 
                   await IsVisibleAsync(ProductsContainer);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取页面标题文本
    /// </summary>
    /// <returns>页面标题</returns>
    public async Task<string> GetPageTitleAsync()
    {
        await WaitForElementAsync(ProductsTitle);
        return await GetTextAsync(ProductsTitle);
    }

    /// <summary>
    /// 检查购物车链接是否可见
    /// </summary>
    /// <returns>购物车链接是否可见</returns>
    public async Task<bool> IsShoppingCartVisibleAsync()
    {
        return await IsVisibleAsync(ShoppingCartLink);
    }

    /// <summary>
    /// 检查菜单按钮是否可见
    /// </summary>
    /// <returns>菜单按钮是否可见</returns>
    public async Task<bool> IsMenuButtonVisibleAsync()
    {
        return await IsVisibleAsync(MenuButton);
    }

    /// <summary>
    /// 获取产品数量
    /// </summary>
    /// <returns>产品数量</returns>
    public async Task<int> GetProductCountAsync()
    {
        await WaitForElementAsync(InventoryItems);
        var products = await Page.QuerySelectorAllAsync(InventoryItems);
        return products.Count;
    }

    /// <summary>
    /// 检查应用Logo是否可见
    /// </summary>
    /// <returns>应用Logo是否可见</returns>
    public async Task<bool> IsAppLogoVisibleAsync()
    {
        return await IsVisibleAsync(AppLogo);
    }

    /// <summary>
    /// 验证登录成功的所有关键元素
    /// </summary>
    /// <returns>登录是否成功</returns>
    public async Task<bool> VerifySuccessfulLoginAsync()
    {
        return await IsOnProductsPageAsync() && 
               await IsShoppingCartVisibleAsync() && 
               await IsMenuButtonVisibleAsync() && 
               await IsAppLogoVisibleAsync() &&
               GetCurrentUrl().Contains("inventory.html");
    }
}