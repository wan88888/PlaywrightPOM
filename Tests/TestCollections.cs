using Xunit;
using PlaywrightPOM.Utils;

namespace PlaywrightPOM.Tests;

/// <summary>
/// 定义测试集合，用于控制测试的并行执行
/// </summary>
[CollectionDefinition("LoginTests")]
public class LoginTestCollection : ICollectionFixture<LoginTestFixture>
{
    // 这个类不需要任何代码，只是用来定义集合
}

[CollectionDefinition("ProductTests")]
public class ProductTestCollection : ICollectionFixture<ProductTestFixture>
{
    // 这个类不需要任何代码，只是用来定义集合
}

/// <summary>
/// 登录测试的共享资源
/// </summary>
public class LoginTestFixture : IDisposable
{
    public LoginTestFixture()
    {
        // 初始化共享资源
        AppConfigurationManager.Initialize();
    }

    public void Dispose()
    {
        // 清理共享资源
    }
}

/// <summary>
/// 产品测试的共享资源
/// </summary>
public class ProductTestFixture : IDisposable
{
    public ProductTestFixture()
    {
        // 初始化共享资源
        AppConfigurationManager.Initialize();
    }

    public void Dispose()
    {
        // 清理共享资源
    }
}