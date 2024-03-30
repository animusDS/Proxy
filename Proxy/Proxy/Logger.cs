using System.Diagnostics;
using System.Reflection;

namespace Proxy;

public class Logger {
    private static readonly string CurrentDir = Directory.GetCurrentDirectory();
    private static readonly string LogDir = $"/logs/{Process.GetCurrentProcess().ProcessName}/";

    private static readonly object ConsoleLock = new();
    private readonly string _loggerName;

    public Logger(MemberInfo type)
        : this(type.Name) {
    }

    public Logger(string name)
        => _loggerName = name;

    static Logger() {
        foreach (var obj in Enum.GetValues(typeof(LogLevel))) {
            if (obj is not LogLevel level)
                continue;

            var path = $"{CurrentDir}{LogDir}{level.ToString().ToLower()}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

    private static void Log(string text, LogLevel level, bool saveToFile, string loggerName) {
#if !DEBUG
            if (level == LogLevel.Debug)
                return;
#endif

        var lvl = level.ToString().ToUpper();
        var lvlPad = lvl.Length + (7 - lvl.Length);
        var senderPad = loggerName.Length + (15 - loggerName.Length);

        text = $"{DateTime.Now.TimeOfDay}  {lvl.PadRight(lvlPad) + loggerName.PadRight(senderPad) + text}";

        lock (ConsoleLock) {
            Console.BackgroundColor = GetBackColor(level);
            Console.ForegroundColor = GetForeColor(level);
            Console.WriteLine(text);

            if (!saveToFile)
                return;

            var path = $"{CurrentDir}{LogDir}{level.ToString().ToLower()}/log.txt";
            using StreamWriter fs = new StreamWriter(path, true);
            fs.WriteLine(text);
        }
    }

    public static void Info(object obj, string loggerName = "Logger", bool saveToFile = false)
        => Log(obj.ToString(), LogLevel.Info, saveToFile, loggerName);

    public static void Debug(object obj, string loggerName = "Logger")
        => Log(obj.ToString(), LogLevel.Debug, false, loggerName);

    public static void Warn(object obj, string loggerName = "Logger")
        => Log(obj.ToString(), LogLevel.Warn, false, loggerName);

    public static void Error(object obj, string loggerName = "Logger")
        => Log(obj.ToString(), LogLevel.Error, false, loggerName);

    public static void Fatal(object obj, string loggerName = "Logger")
        => Log(obj.ToString(), LogLevel.Fatal, false, loggerName);

    public void Info(object obj, bool saveToFile = true)
        => Log(obj.ToString(), LogLevel.Info, saveToFile, _loggerName);

    public void Debug(object obj, bool saveToFile = false)
        => Log(obj.ToString(), LogLevel.Debug, saveToFile, _loggerName);

    public void Warn(object obj, bool saveToFile = true)
        => Log(obj.ToString(), LogLevel.Warn, saveToFile, _loggerName);

    public void Error(object obj, bool saveToFile = true)
        => Log(obj.ToString(), LogLevel.Error, saveToFile, _loggerName);

    public void Fatal(object obj, bool saveToFile = true)
        => Log(obj.ToString(), LogLevel.Fatal, saveToFile, _loggerName);

    private static ConsoleColor GetBackColor(LogLevel level) {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            return ConsoleColor.Black;

        switch (level) {
            case LogLevel.Info:
            case LogLevel.Debug:
            case LogLevel.Warn:
            case LogLevel.Fatal:
            case LogLevel.Error: // Red kinda yucky
                return ConsoleColor.Black;
            default:
                throw new ArgumentException($"Invalid LogLevel '{level}'");
        }
    }

    private static ConsoleColor GetForeColor(LogLevel level) {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            return ConsoleColor.Gray;

        return level switch {
            LogLevel.Info => ConsoleColor.Gray,
            LogLevel.Debug => ConsoleColor.DarkGray,
            LogLevel.Warn => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.White,
            LogLevel.Fatal => ConsoleColor.White,
            _ => throw new ArgumentException($"Invalid LogLevel '{level}'"),
        };
    }
}

public enum LogLevel {
    Info,
    Debug,
    Warn,
    Error,
    Fatal,
}