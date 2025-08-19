using Microsoft.Playwright;

namespace PlaywrightPOM.Utils;

public class PlaywrightDriver : IPlaywrightDriver
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private readonly object _lock = new object();
    private bool _disposed = false;
    private readonly string _instanceId;

    public IPage Page => _page ?? throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");
    public IBrowserContext Context => _context ?? throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");
    public string InstanceId => _instanceId;

    public PlaywrightDriver()
    {
        _instanceId = Guid.NewGuid().ToString("N")[..8];
    }

    public async Task InitializeAsync(string browserType = "chromium", bool headless = true)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(PlaywrightDriver));

        lock (_lock)
        {
            if (_playwright != null)
                return; // 已经初始化
        }

        try
        {
            _playwright = await Playwright.CreateAsync();
            
            // 使用配置管理器获取浏览器配置
            var browserConfig = AppConfigurationManager.GetBrowserConfig();
            var viewportWidth = browserConfig.ViewportWidth;
            var viewportHeight = browserConfig.ViewportHeight;
            var timeout = browserConfig.Timeout;
            
            _browser = browserType.ToLower() switch
            {
                "chromium" => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
                { 
                    Headless = headless,
                    Timeout = timeout
                }),
                "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions 
                { 
                    Headless = headless,
                    Timeout = timeout
                }),
                "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions 
                { 
                    Headless = headless,
                    Timeout = timeout
                }),
                _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
            };

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = viewportWidth, Height = viewportHeight }
            });

            _page = await _context.NewPageAsync();
            
            // 设置默认超时
            _page.SetDefaultTimeout(timeout);
        }
        catch
        {
            await CloseAsync();
            throw;
        }
    }

    public async Task<IPage> NewPageAsync()
    {
        if (_context == null)
            throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");
            
        return await _context.NewPageAsync();
    }

    public async Task CloseAsync()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
            _page = null;
        }

        if (_context != null)
        {
            await _context.CloseAsync();
            _context = null;
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
    }

    public void Dispose()
    {
        CloseAsync().GetAwaiter().GetResult();
    }
}