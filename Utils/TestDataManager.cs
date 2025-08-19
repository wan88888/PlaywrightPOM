using Newtonsoft.Json;
using System.Reflection;

namespace PlaywrightPOM.Utils;

/// <summary>
/// 测试数据管理器，用于读取和管理JSON测试数据
/// </summary>
public static class TestDataManager
{
    private static readonly string TestDataPath = Path.Combine(GetProjectRoot(), "TestData");
    
    /// <summary>
    /// 获取项目根目录
    /// </summary>
    /// <returns>项目根目录路径</returns>
    private static string GetProjectRoot()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyLocation = assembly.Location;
        var directory = new DirectoryInfo(Path.GetDirectoryName(assemblyLocation)!);
        
        // 向上查找直到找到包含TestData文件夹的目录
        while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "TestData")))
        {
            directory = directory.Parent;
        }
        
        return directory?.FullName ?? throw new DirectoryNotFoundException("无法找到TestData目录");
    }
    
    /// <summary>
    /// 读取JSON文件并反序列化为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="fileName">文件名（不包含扩展名）</param>
    /// <returns>反序列化后的对象</returns>
    public static T ReadJsonData<T>(string fileName)
    {
        var filePath = Path.Combine(TestDataPath, $"{fileName}.json");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"测试数据文件不存在: {filePath}");
        }
        
        var jsonContent = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<T>(jsonContent);
        
        return data ?? throw new InvalidOperationException($"无法反序列化文件: {filePath}");
    }
    
    /// <summary>
    /// 获取用户测试数据
    /// </summary>
    /// <returns>用户数据对象</returns>
    public static UserTestData GetUserData()
    {
        return ReadJsonData<UserTestData>("users");
    }
    
    /// <summary>
    /// 获取产品测试数据
    /// </summary>
    /// <returns>产品数据对象</returns>
    public static ProductTestData GetProductData()
    {
        return ReadJsonData<ProductTestData>("products");
    }
    
    /// <summary>
    /// 获取有效用户列表
    /// </summary>
    /// <returns>有效用户列表</returns>
    public static List<UserData> GetValidUsers()
    {
        return GetUserData().ValidUsers;
    }
    
    /// <summary>
    /// 获取无效用户列表
    /// </summary>
    /// <returns>无效用户列表</returns>
    public static List<UserData> GetInvalidUsers()
    {
        return GetUserData().InvalidUsers;
    }
    
    /// <summary>
    /// 获取产品列表
    /// </summary>
    /// <returns>产品列表</returns>
    public static List<ProductData> GetProducts()
    {
        return GetProductData().Products;
    }
    
    /// <summary>
    /// 获取测试场景列表
    /// </summary>
    /// <returns>测试场景列表</returns>
    public static List<TestScenario> GetTestScenarios()
    {
        return GetProductData().TestScenarios;
    }
    
    /// <summary>
    /// 根据用户名获取用户数据
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns>用户数据，如果未找到则返回null</returns>
    public static UserData? GetUserByUsername(string username)
    {
        var userData = GetUserData();
        return userData.ValidUsers.FirstOrDefault(u => u.Username == username) ??
               userData.InvalidUsers.FirstOrDefault(u => u.Username == username);
    }
    
    /// <summary>
    /// 根据产品ID获取产品数据
    /// </summary>
    /// <param name="productId">产品ID</param>
    /// <returns>产品数据，如果未找到则返回null</returns>
    public static ProductData? GetProductById(string productId)
    {
        return GetProducts().FirstOrDefault(p => p.Id == productId);
    }
}

/// <summary>
/// 用户测试数据结构
/// </summary>
public class UserTestData
{
    [JsonProperty("validUsers")]
    public List<UserData> ValidUsers { get; set; } = new();
    
    [JsonProperty("invalidUsers")]
    public List<UserData> InvalidUsers { get; set; } = new();
}

/// <summary>
/// 用户数据结构
/// </summary>
public class UserData
{
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("expectedResult")]
    public string ExpectedResult { get; set; } = string.Empty;
    
    [JsonProperty("expectedMessage")]
    public string? ExpectedMessage { get; set; }
}

/// <summary>
/// 产品测试数据结构
/// </summary>
public class ProductTestData
{
    [JsonProperty("products")]
    public List<ProductData> Products { get; set; } = new();
    
    [JsonProperty("testScenarios")]
    public List<TestScenario> TestScenarios { get; set; } = new();
}

/// <summary>
/// 产品数据结构
/// </summary>
public class ProductData
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;
    
    [JsonProperty("category")]
    public string Category { get; set; } = string.Empty;
    
    [JsonProperty("inStock")]
    public bool InStock { get; set; }
}

/// <summary>
/// 测试场景数据结构
/// </summary>
public class TestScenario
{
    [JsonProperty("scenario")]
    public string Scenario { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("products")]
    public List<string>? Products { get; set; }
    
    [JsonProperty("expectedCartCount")]
    public int? ExpectedCartCount { get; set; }
    
    [JsonProperty("addProducts")]
    public List<string>? AddProducts { get; set; }
    
    [JsonProperty("removeProducts")]
    public List<string>? RemoveProducts { get; set; }
    
    [JsonProperty("sortOptions")]
    public List<SortOption>? SortOptions { get; set; }
}

/// <summary>
/// 排序选项数据结构
/// </summary>
public class SortOption
{
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("expectedFirst")]
    public string ExpectedFirst { get; set; } = string.Empty;
}