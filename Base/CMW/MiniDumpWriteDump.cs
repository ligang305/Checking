using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// Mini dump file generation utility class
/// Used to generate diagnostic dump files when the program crashes
/// </summary>
public static class DumpWriter
{
    // Default configuration
    private static string _defaultDumpDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashDumps");
    private static string _defaultProductName = Process.GetCurrentProcess().ProcessName;
    private static MINIDUMP_TYPE _defaultDumpType = MINIDUMP_TYPE.MiniDumpWithDataSegs | MINIDUMP_TYPE.MiniDumpWithHandleData;

    /// <summary>
    /// Initialize default configuration
    /// </summary>
    /// <param name="defaultDumpDirectory">Default dump file directory</param>
    /// <param name="defaultProductName">Default product name</param>
    /// <param name="defaultDumpType">Default dump type</param>
    public static void Initialize(string defaultDumpDirectory = null, string defaultProductName = null, MINIDUMP_TYPE? defaultDumpType = null)
    {
        if (!string.IsNullOrEmpty(defaultDumpDirectory))
            _defaultDumpDirectory = defaultDumpDirectory;

        if (!string.IsNullOrEmpty(defaultProductName))
            _defaultProductName = defaultProductName;

        if (defaultDumpType.HasValue)
            _defaultDumpType = defaultDumpType.Value;
    }

    [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool MiniDumpWriteDump(
        IntPtr hProcess,
        uint processId,
        SafeHandle hFile,
        MINIDUMP_TYPE dumpType,
        IntPtr exceptionParam,
        IntPtr userStreamParam,
        IntPtr callbackParam);

    [Flags]
    public enum MINIDUMP_TYPE : uint
    {
        MiniDumpNormal = 0x00000000,
        MiniDumpWithDataSegs = 0x00000001,
        MiniDumpWithFullMemory = 0x00000002,
        MiniDumpWithHandleData = 0x00000004,
        MiniDumpFilterMemory = 0x00000008,
        MiniDumpScanMemory = 0x00000010,
        MiniDumpWithUnloadedModules = 0x00000020,
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
        MiniDumpFilterModulePaths = 0x00000080,
        MiniDumpWithProcessThreadData = 0x00000100,
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        MiniDumpWithoutOptionalData = 0x00000400,
        MiniDumpWithFullMemoryInfo = 0x00000800,
        MiniDumpWithThreadInfo = 0x00001000,
        MiniDumpWithCodeSegs = 0x00002000,
        MiniDumpWithoutAuxiliaryState = 0x00004000,
        MiniDumpWithFullAuxiliaryState = 0x00008000,
        MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
        MiniDumpIgnoreInaccessibleMemory = 0x00020000,
        MiniDumpWithTokenInformation = 0x00040000,
        MiniDumpWithModuleHeaders = 0x00080000,
        MiniDumpFilterTriage = 0x00100000,
        MiniDumpValidTypeFlags = 0x001fffff
    }

    /// <summary>
    /// Create dump file (using default configuration)
    /// </summary>
    /// <param name="context">Context information for file name identification</param>
    /// <param name="logger">Optional log recording Action</param>
    /// <returns>Dump file path, returns null if failed</returns>
    public static string CreateDump(string context, Action<string> logger = null)
    {
        return CreateDump(context, _defaultDumpDirectory, _defaultProductName, _defaultDumpType, logger);
    }

    /// <summary>
    /// Create dump file (fully customizable configuration)
    /// </summary>
    /// <param name="context">Context information for file name identification</param>
    /// <param name="dumpDirectory">Dump file directory</param>
    /// <param name="productName">Product name for file naming</param>
    /// <param name="dumpType">Dump type</param>
    /// <param name="logger">Optional log recording Action</param>
    /// <param name="process">Process to dump, current process by default</param>
    /// <returns>Dump file path, returns null if failed</returns>
    public static string CreateDump(string context, string dumpDirectory, string productName,
                                  MINIDUMP_TYPE dumpType, Action<string> logger = null, Process process = null)
    {
        if (process == null)
            process = Process.GetCurrentProcess();

        try
        {
            // Ensure directory exists
            Directory.CreateDirectory(dumpDirectory);

            // Generate file name: ProductName_Context_Timestamp_PID.dmp
            string fileName = $"{productName}_{context}_{DateTime.Now:yyyyMMdd_HHmmss}_{process.Id}.dmp";
            string dumpFilePath = Path.Combine(dumpDirectory, fileName);

            Log(logger, $"Starting to create dump file: {dumpFilePath}");

            using (var fs = new FileStream(dumpFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bool success = MiniDumpWriteDump(
                    process.Handle,
                    (uint)process.Id,
                    fs.SafeFileHandle,
                    dumpType,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero);

                if (success)
                {
                    Log(logger, $"Dump file created successfully: {dumpFilePath}");
                    return dumpFilePath;
                }
                else
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    string errorMessage = GetErrorMessage(errorCode);
                    Log(logger, $"Failed to create dump file, error code: {errorCode} - {errorMessage}");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Log(logger, $"Exception occurred while creating dump file: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Create dump file for specific exception (automatic exception type recognition)
    /// </summary>
    /// <param name="exception">Exception object</param>
    /// <param name="logger">Optional log recording Action</param>
    /// <returns>Dump file path, returns null if failed</returns>
    public static string CreateDumpForException(Exception exception, Action<string> logger = null)
    {
        string context = "Exception";

        if (exception != null)
        {
            context = exception.GetType().Name;

            // Special handling for native exceptions
            if (exception is SEHException sehEx)
            {
                context += $"_0x{sehEx.ErrorCode:X8}";
            }
        }

        return CreateDump(context, logger);
    }

    /// <summary>
    /// Get system error message
    /// </summary>
    private static string GetErrorMessage(int errorCode)
    {
        try
        {
            return new System.ComponentModel.Win32Exception(errorCode).Message;
        }
        catch
        {
            return "Unknown error";
        }
    }

    /// <summary>
    /// Log message
    /// </summary>
    private static void Log(Action<string> logger, string message)
    {
        logger?.Invoke($"[DumpWriter] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }
   
    /// <summary>
    /// 获取SEH异常的名称描述
    /// </summary>
    public static string GetSEHExceptionName(int errorCode)
    {
        switch (errorCode)
        {
            case -1073740791:
                return "STATUS_STACK_BUFFER_OVERRUN";         // 0xC0000409
            case -1073741676:
                return "STATUS_ACCESS_VIOLATION";            // 0xC0000005
            case -1073741819:
                return "STATUS_INTEGER_DIVIDE_BY_ZERO";      // 0xC0000094
            case -1073741811:
                return "STATUS_ARRAY_BOUNDS_EXCEEDED";       // 0xC000008C
            case -1073740940:
                return "STATUS_INVALID_HANDLE";              // 0xC0000008
            case -1073740766:
                return "STATUS_HEAP_CORRUPTION";             // 0xC0000374
            case -1073740786:
                return "STATUS_STACK_OVERFLOW";              // 0xC00000FD
            case -1073741510:
                return "STATUS_DLL_NOT_FOUND";               // 0xC0000135
            case -1073741511:
                return "STATUS_ORDINAL_NOT_FOUND";           // 0xC0000136
            case -1073741512:
                return "STATUS_ENTRYPOINT_NOT_FOUND";        // 0xC0000137
            case -1073741812:
                return "STATUS_FLOAT_DIVIDE_BY_ZERO";        // 0xC000008E
            case -1073741813:
                return "STATUS_FLOAT_OVERFLOW";              // 0xC000008F
            case -1073741814:
                return "STATUS_FLOAT_UNDERFLOW";             // 0xC0000090
            case -1073741815:
                return "STATUS_FLOAT_INEXACT_RESULT";        // 0xC0000091
            case -1073741816:
                return "STATUS_FLOAT_INVALID_OPERATION";     // 0xC0000092
            case -1073741817:
                return "STATUS_FLOAT_STACK_CHECK";           // 0xC0000093
            case -1073741790:
                return "STATUS_ILLEGAL_INSTRUCTION";         // 0xC000001D
            case -1073740767:
                return "STATUS_HEAP_CORRUPTION";             // 0xC0000374
            case -1073740787:
                return "STATUS_STACK_OVERFLOW_READ";         // 0xC00000FD
            default:
                return $"UNKNOWN_SEH_EXCEPTION_0x{errorCode:X8}";
        }
    }

    /// <summary>
    /// 判断是否为关键的原生异常（需要生成转储文件）
    /// </summary>
    public static bool IsCriticalNativeException(int errorCode)
    {
        // 这些异常通常表示严重的内存损坏或程序逻辑错误
        switch (errorCode)
        {
            case -1073740791: // STATUS_STACK_BUFFER_OVERRUN
            case -1073741676: // STATUS_ACCESS_VIOLATION
            case -1073741819: // STATUS_INTEGER_DIVIDE_BY_ZERO
            case -1073741811: // STATUS_ARRAY_BOUNDS_EXCEEDED
            case -1073740766: // STATUS_HEAP_CORRUPTION
            case -1073740786: // STATUS_STACK_OVERFLOW
            case -1073741790: // STATUS_ILLEGAL_INSTRUCTION
            case -1073740940: // STATUS_INVALID_HANDLE
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 判断是否为浮点异常（通常不太严重）
    /// </summary>
    public static bool IsFloatingPointException(int errorCode)
    {
        switch (errorCode)
        {
            case -1073741812: // STATUS_FLOAT_DIVIDE_BY_ZERO
            case -1073741813: // STATUS_FLOAT_OVERFLOW
            case -1073741814: // STATUS_FLOAT_UNDERFLOW
            case -1073741815: // STATUS_FLOAT_INEXACT_RESULT
            case -1073741816: // STATUS_FLOAT_INVALID_OPERATION
            case -1073741817: // STATUS_FLOAT_STACK_CHECK
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 判断是否为DLL相关异常
    /// </summary>
    public static bool IsDllRelatedException(int errorCode)
    {
        switch (errorCode)
        {
            case -1073741510: // STATUS_DLL_NOT_FOUND
            case -1073741511: // STATUS_ORDINAL_NOT_FOUND
            case -1073741512: // STATUS_ENTRYPOINT_NOT_FOUND
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// 额外的分类方法
    /// </summary>
    public static bool IsCriticalMemoryException(int errorCode)
    {
        switch (errorCode)
        {
            case -1073741676: // ACCESS_VIOLATION
            case -1073740766: // HEAP_CORRUPTION
            case -1073740786: // STACK_OVERFLOW
            case -1073741811: // ARRAY_BOUNDS_EXCEEDED
                return true;
            default:
                return false;
        }
    }

    public static bool IsArithmeticException(int errorCode)
    {
        switch (errorCode)
        {
            case -1073741819: // INTEGER_DIVIDE_BY_ZERO
            case -1073741812: // FLOAT_DIVIDE_BY_ZERO
            case -1073741813: // FLOAT_OVERFLOW
            case -1073741814: // FLOAT_UNDERFLOW
                return true;
            default:
                return false;
        }
    }
    /// <summary>
    /// Get recommended dump type (balances file size and information completeness)
    /// </summary>
    public static MINIDUMP_TYPE RecommendedDumpType =>
        MINIDUMP_TYPE.MiniDumpWithDataSegs |
        MINIDUMP_TYPE.MiniDumpWithHandleData |
        MINIDUMP_TYPE.MiniDumpWithUnloadedModules;

    /// <summary>
    /// Get full memory dump type (larger file but contains most information)
    /// </summary>
    public static MINIDUMP_TYPE FullMemoryDumpType =>
        MINIDUMP_TYPE.MiniDumpWithFullMemory |
        MINIDUMP_TYPE.MiniDumpWithHandleData |
        MINIDUMP_TYPE.MiniDumpWithThreadInfo;
}


public class ProcdumpConfiguration
{
    public bool Enabled { get; set; } = true;
    public string ProcdumpPath { get; set; }
    public string DumpDirectory { get; set; } = @"C:\Dumps";
    public string[] ExceptionCodes { get; set; } = new[] { "C0000409" };
    public bool AcceptEula { get; set; } = true;
    // 新增：用于标记监控进程的命令行参数
    public string AdditionalArguments { get; set; } = "";
    public bool CreateNoWindow { get; set; } = true;
    public int DumpCount { get; set; } = 3; // 最大转储数量
    public bool FullMemoryDump { get; set; } = true; // 是否生成完整内存转储

    public ProcdumpConfiguration()
    {
        ProcdumpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "procdump64.exe");
    }
   
}



public class ProcdumpManager : IDisposable
{
    private Process _procdumpProcess;
    private ProcdumpConfiguration _config;
    private bool _isMonitoring = false;
    private readonly object _lockObject = new object();

    public event Action<string> LogMessage;
    public event Action<string> ErrorOccurred;

    public ProcdumpManager(ProcdumpConfiguration config = null)
    {
        _config = config ?? new ProcdumpConfiguration();
    }

    public bool StartMonitoring()
    {
        lock (_lockObject)
        {
            if (_isMonitoring)
            {
                Log("Procdump 监控已在运行");
                return true;
            }

            if (!_config.Enabled)
            {
                Log("Procdump 监控已禁用");
                return false;
            }

            if (!File.Exists(_config.ProcdumpPath))
            {
                Error($"未找到 Procdump: {_config.ProcdumpPath}");
                return false;
            }

            try
            {
                var currentProcess = Process.GetCurrentProcess();

                // 确保 dump 目录存在
                Directory.CreateDirectory(_config.DumpDirectory);

                string arguments = BuildArguments(currentProcess.Id);

                var startInfo = new ProcessStartInfo
                {
                    FileName = _config.ProcdumpPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = _config.CreateNoWindow,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                _procdumpProcess = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                _procdumpProcess.OutputDataReceived += OnOutputReceived;
                _procdumpProcess.ErrorDataReceived += OnErrorReceived;
                _procdumpProcess.Exited += OnProcessExited;

                if (_procdumpProcess.Start())
                {
                    _procdumpProcess.BeginOutputReadLine();
                    _procdumpProcess.BeginErrorReadLine();

                    _isMonitoring = true;
                    Log($"Procdump 启动成功，目标PID: {currentProcess.Id}");
                    Log($"转储目录: {_config.DumpDirectory}");
                    Log($"命令行: {_config.ProcdumpPath} {arguments}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error($"启动 Procdump 失败: {ex.Message}");
            }

            return false;
        }
    }
    private string BuildArguments(int targetProcessId)
    {
        var args = new List<string>();

        // 1. 必需参数
        if (_config.AcceptEula)
            args.Add("-accepteula");

        // 2. 监控配置
        args.Add("-e"); // 监控未处理异常
        if (_config.DumpCount > 0)
            args.Add($"-n {_config.DumpCount}");

        // 3. 异常过滤（正确的语法）
        if (_config.ExceptionCodes != null && _config.ExceptionCodes.Length > 0)
        {
            // 将异常代码用逗号连接
            string exceptionCodes = string.Join(",", _config.ExceptionCodes);
            args.Add($"-f {exceptionCodes}");
        }

        // 4. 转储类型
        if (_config.FullMemoryDump)
            args.Add("-ma");

        // 5. 输出选项
        args.Add("-o"); // 覆盖现有文件

        // 6. 关键部分：先指定PID，然后是转储目录
        args.Add(targetProcessId.ToString());
        args.Add(_config.DumpDirectory);

        return string.Join(" ", args);
    }

    public void StopMonitoring()
    {
        lock (_lockObject)
        {
            if (!_isMonitoring) return;

            try
            {
                if (_procdumpProcess != null && !_procdumpProcess.HasExited)
                {
                    _procdumpProcess.Kill();
                    if (!_procdumpProcess.WaitForExit(5000))
                    {
                        Error("Procdump 进程未能正常退出");
                    }
                }
            }
            catch (Exception ex)
            {
                Error($"停止 Procdump 时出错: {ex.Message}");
            }
            finally
            {
                _isMonitoring = false;
                _procdumpProcess?.Dispose();
                _procdumpProcess = null;
                Log("Procdump 监控已停止");
            }
        }
    }

    private void OnOutputReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            //Log($"[Procdump] {e.Data}");
        }
    }

    private void OnErrorReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            Error($"[Procdump Error] {e.Data}");
        }
    }

    private void OnProcessExited(object sender, EventArgs e)
    {
        lock (_lockObject)
        {
            _isMonitoring = false;
            var exitCode = _procdumpProcess?.ExitCode ?? -1;

            Log($"Procdump 进程退出，代码: {exitCode}");

            _procdumpProcess?.Dispose();
            _procdumpProcess = null;
        }
    }

    private void Log(string message)
    {
        LogMessage?.Invoke($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        Console.WriteLine($"[INFO] {message}");
    }

    private void Error(string message)
    {
        ErrorOccurred?.Invoke(message);
        Console.WriteLine($"[ERROR] {message}");
    }

    public void Dispose()
    {
        StopMonitoring();
        GC.SuppressFinalize(this);
    }
}
