using Microsoft.Playwright;

namespace PlaywrightPOM.Utils;

public interface IPlaywrightDriver : IDisposable
{
    IPage Page { get; }
    IBrowser Browser { get; }
    IBrowserContext Context { get; }
    
    Task InitializeAsync(string browserType = "chromium", bool headless = true);
    Task<IPage> NewPageAsync();
    Task CloseAsync();
}