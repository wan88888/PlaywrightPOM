using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Diagnostics;

namespace PlaywrightPOM.Utils
{
    /// <summary>
    /// ExtentReports 报告管理器
    /// </summary>
    public static class ExtentReportManager
    {
        private static ExtentReports? _extent;
        private static ExtentSparkReporter? _sparkReporter;
        private static readonly object _lock = new object();
        
        /// <summary>
        /// 获取ExtentReports实例
        /// </summary>
        public static ExtentReports GetExtentReports()
        {
            if (_extent == null)
            {
                lock (_lock)
                {
                    if (_extent == null)
                    {
                        InitializeReports();
                    }
                }
            }
            return _extent!;
        }
        
        /// <summary>
        /// 初始化ExtentReports
        /// </summary>
        public static void InitializeReports()
        {
            var reportsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtentReports");
            Directory.CreateDirectory(reportsPath);
            
            var reportPath = Path.Combine(reportsPath, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            
_sparkReporter = new ExtentSparkReporter(reportPath);
            
            // 配置Spark报告
            _sparkReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard;
            _sparkReporter.Config.DocumentTitle = "Playwright POM 自动化测试报告";
            _sparkReporter.Config.ReportName = "Web自动化测试执行报告";
            
            _extent = new ExtentReports();
            _extent.AttachReporter(_sparkReporter);
            
            // 添加系统信息
            _extent.AddSystemInfo("测试环境", "SauceDemo");
            _extent.AddSystemInfo("操作系统", Environment.OSVersion.ToString());
            _extent.AddSystemInfo(".NET版本", Environment.Version.ToString());
            _extent.AddSystemInfo("测试框架", "Playwright + xUnit");
            _extent.AddSystemInfo("浏览器", TestConfig.Browser.DefaultBrowserType);
            _extent.AddSystemInfo("执行时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            Console.WriteLine($"ExtentReports 初始化完成，报告路径: {reportPath}");
        }
        
        /// <summary>
        /// 创建测试用例
        /// </summary>
        /// <param name="testName">测试名称</param>
        /// <param name="description">测试描述</param>
        /// <returns>ExtentTest实例</returns>
        public static ExtentTest CreateTest(string testName, string description = "")
        {
            if (_extent == null)
                InitializeReports();
                
            return _extent!.CreateTest(testName, description);
        }
        
        /// <summary>
        /// 刷新报告
        /// </summary>
        public static void FlushReports()
        {
            _extent?.Flush();
        }
        
        /// <summary>
        /// 添加截图到测试报告
        /// </summary>
        /// <param name="test">ExtentTest实例</param>
        /// <param name="screenshotPath">截图路径</param>
        /// <param name="title">截图标题</param>
        public static void AddScreenshot(ExtentTest test, string screenshotPath, string title = "截图")
        {
            if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
            {
                test.AddScreenCaptureFromPath(screenshotPath, title);
            }
        }
        
        /// <summary>
        /// 记录测试步骤
        /// </summary>
        /// <param name="test">ExtentTest实例</param>
        /// <param name="status">状态</param>
        /// <param name="details">详细信息</param>
        public static void LogStep(ExtentTest test, Status status, string details)
        {
            test.Log(status, details);
        }
        
        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="test">ExtentTest实例</param>
        /// <param name="exception">异常</param>
        public static void LogException(ExtentTest test, Exception exception)
        {
            test.Log(Status.Fail, exception.ToString());
        }
        
        /// <summary>
        /// 设置测试分类
        /// </summary>
        /// <param name="test">ExtentTest实例</param>
        /// <param name="categories">分类标签</param>
        public static void AssignCategory(ExtentTest test, params string[] categories)
        {
            test.AssignCategory(categories);
        }
        
        /// <summary>
        /// 设置测试作者
        /// </summary>
        /// <param name="test">ExtentTest实例</param>
        /// <param name="authors">作者</param>
        public static void AssignAuthor(ExtentTest test, params string[] authors)
        {
            test.AssignAuthor(authors);
        }
        
        /// <summary>
        /// 获取报告路径
        /// </summary>
        /// <returns>报告文件路径</returns>
        public static string GetReportPath()
        {
            var reportsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtentReports");
            return Path.Combine(reportsPath, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        }
        
        /// <summary>
        /// 打开报告文件
        /// </summary>
        public static void OpenReport()
        {
            var reportPath = GetReportPath();
            if (!string.IsNullOrEmpty(reportPath) && File.Exists(reportPath))
            {
                try
                {
                    if (OperatingSystem.IsMacOS())
                    {
                        System.Diagnostics.Process.Start("open", reportPath);
                    }
                    else if (OperatingSystem.IsWindows())
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = reportPath,
                            UseShellExecute = true
                        });
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        System.Diagnostics.Process.Start("xdg-open", reportPath);
                    }
                    
                    Console.WriteLine($"已尝试打开测试报告: {reportPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法打开报告文件: {ex.Message}");
                }
            }
        }
    }
}