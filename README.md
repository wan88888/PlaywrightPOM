# Playwright POM 自动化测试框架

## 📖 项目简介

这是一个基于 Playwright 和 C# 的现代化 Web 自动化测试框架，采用页面对象模型（POM）设计模式，支持数据驱动测试、多环境配置、并发测试执行和详细的测试报告。

## 🏗️ 项目架构

```
PlaywrightPOM/
├── Pages/                    # 页面对象模型
│   ├── BasePage.cs          # 基础页面类
│   ├── LoginPage.cs         # 登录页面
│   └── ProductsPage.cs      # 产品页面
├── Tests/                   # 测试用例
│   ├── BaseTest.cs          # 基础测试类
│   ├── LoginTest.cs         # 登录测试
│   ├── DataDrivenLoginTest.cs # 数据驱动登录测试
│   └── TestCollections.cs   # 测试集合配置
├── Utils/                   # 工具类
│   ├── ConfigurationManager.cs    # 配置管理器
│   ├── PlaywrightDriver.cs        # Playwright 驱动封装
│   ├── ExtentReportManager.cs     # 测试报告管理
│   ├── TestDataManager.cs         # 测试数据管理
│   └── TestConfig.cs              # 测试配置
├── TestData/                # 测试数据
│   ├── users.json          # 用户测试数据
│   └── products.json       # 产品测试数据
├── appsettings.json         # 默认配置
├── appsettings.Test.json    # 测试环境配置
└── appsettings.Production.json # 生产环境配置
```

## ✨ 核心特性

### 🎯 页面对象模型（POM）
- **BasePage**: 提供通用的页面操作方法
- **LoginPage**: 封装登录页面的元素和操作
- **ProductsPage**: 封装产品页面的元素和操作

### 🔧 配置管理
- **多环境支持**: 开发、测试、生产环境配置
- **外部化配置**: 通过 JSON 文件管理所有配置项
- **环境变量**: 支持通过 `TEST_ENVIRONMENT` 环境变量切换环境

### 📊 数据驱动测试
- **JSON 数据源**: 测试数据存储在 JSON 文件中
- **动态测试生成**: 使用 xUnit 的 `[Theory]` 和 `[MemberData]` 特性
- **测试数据管理**: 统一的测试数据读取和管理

### 🚀 并发测试支持
- **测试隔离**: 使用 xUnit Collection 实现测试隔离
- **线程安全**: PlaywrightDriver 支持并发执行
- **资源管理**: 自动管理浏览器实例和页面资源

### 📈 测试报告
- **ExtentReports**: 生成详细的 HTML 测试报告
- **截图支持**: 测试失败时自动截图
- **步骤记录**: 详细记录每个测试步骤

## 🚀 快速开始

### 环境要求
- .NET 8.0 或更高版本
- Visual Studio 2022 或 VS Code

### 安装依赖

```bash
# 克隆项目
git clone <repository-url>
cd PlaywrightPOM

# 还原 NuGet 包
dotnet restore

# 安装 Playwright 浏览器
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定测试类
dotnet test --filter "FullyQualifiedName~LoginTest"

# 运行数据驱动测试
dotnet test --filter "FullyQualifiedName~DataDrivenLoginTest"
```

## ⚙️ 配置说明

### 环境配置

通过设置 `TEST_ENVIRONMENT` 环境变量来切换不同环境：

```bash
# Windows
set TEST_ENVIRONMENT=Test

# macOS/Linux
export TEST_ENVIRONMENT=Test
```

### 配置文件结构

```json
{
  "Browser": {
    "DefaultType": "chromium",
    "Headless": true,
    "Timeout": 30000,
    "ViewportWidth": 1920,
    "ViewportHeight": 1080
  },
  "Urls": {
    "SauceDemo": "https://www.saucedemo.com",
    "SauceDemoInventory": "https://www.saucedemo.com/inventory.html"
  },
  "Users": {
    "ValidUsername": "standard_user",
    "ValidPassword": "secret_sauce",
    "InvalidUsername": "invalid_user",
    "InvalidPassword": "invalid_password"
  },
  "Timeouts": {
    "DefaultWait": 10000,
    "PageLoad": 30000,
    "ElementWait": 5000
  }
}
```

## 📝 测试数据管理

### 用户测试数据 (users.json)

```json
{
  "validUsers": [
    {
      "username": "standard_user",
      "password": "secret_sauce",
      "description": "标准用户",
      "expectedResult": "success"
    }
  ],
  "invalidUsers": [
    {
      "username": "locked_out_user",
      "password": "secret_sauce",
      "description": "被锁定的用户",
      "expectedResult": "locked_out",
      "expectedMessage": "Epic sadface: Sorry, this user has been locked out."
    }
  ]
}
```

## 🧪 编写测试用例

### 基础测试示例

```csharp
[Collection("LoginTests")]
public class MyTest : BaseTest
{
    [Fact]
    public async Task TestLogin()
    {
        await InitializeAsync("chromium", true, "TestLogin");
        StartTest("登录测试", "验证用户登录功能");
        
        var loginPage = new LoginPage(Driver);
        await loginPage.NavigateToLoginPageAsync();
        await loginPage.LoginAsync("standard_user", "secret_sauce");
        
        // 验证登录成功
        var productsPage = new ProductsPage(Driver);
        Assert.True(await productsPage.IsOnProductsPageAsync());
        
        await CleanupAsync(true);
    }
}
```

### 数据驱动测试示例

```csharp
[Theory]
[MemberData(nameof(GetTestData))]
public async Task TestDataDriven(UserData userData)
{
    await InitializeAsync("chromium", true, $"Test_{userData.Username}");
    
    var loginPage = new LoginPage(Driver);
    await loginPage.NavigateToLoginPageAsync();
    await loginPage.LoginAsync(userData.Username, userData.Password);
    
    // 根据预期结果进行验证
    if (userData.ExpectedResult == "success")
    {
        var productsPage = new ProductsPage(Driver);
        Assert.True(await productsPage.IsOnProductsPageAsync());
    }
    
    await CleanupAsync(true);
}

public static IEnumerable<object[]> GetTestData()
{
    var users = TestDataManager.GetValidUsers();
    return users.Select(user => new object[] { user });
}
```

## 📊 测试报告

测试完成后，会在项目根目录生成 HTML 格式的测试报告，包含：
- 测试执行概览
- 详细的测试步骤
- 失败测试的截图
- 测试执行时间统计

## 🔧 自定义配置

### 添加新的页面对象

1. 在 `Pages` 目录下创建新的页面类
2. 继承 `BasePage` 类
3. 定义页面元素选择器
4. 实现页面操作方法

### 添加新的测试数据

1. 在 `TestData` 目录下创建 JSON 文件
2. 在 `TestDataManager` 中添加读取方法
3. 在测试类中使用 `[MemberData]` 引用数据

### 扩展配置项

1. 在 `appsettings.json` 中添加新的配置节
2. 在 `AppConfigurationManager` 中添加对应的读取方法
3. 在需要的地方调用配置

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 📞 联系方式

如有问题或建议，请通过以下方式联系：
- 创建 Issue
- 发送邮件
- 提交 Pull Request

---

**Happy Testing! 🎉**