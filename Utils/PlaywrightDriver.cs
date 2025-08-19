using Microsoft.Playwright;

namespace PlaywrightPOM.Utils;

public class PlaywrightDriver : IPlaywrightDriver
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    public IPage Page => _page ?? throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");
    public IBrowserContext Context => _context ?? throw new InvalidOperationException("Driver not initialized. Call InitializeAsync first.");

    public async Task InitializeAsync(string browserType = "chromium", bool headless = true)
    {
        _playwright = await Playwright.CreateAsync();
        
        _browser = browserType.ToLower() switch
        {
            "chromium" => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless }),
            "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless }),
            "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless }),
            _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
        };

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        _page = await _context.NewPageAsync();
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