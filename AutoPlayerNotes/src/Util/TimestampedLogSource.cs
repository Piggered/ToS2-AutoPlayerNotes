using System;
using System.Text;
using BepInEx.Logging;

// ReSharper disable UnusedMember.Global

namespace AutoPlayerNotes.Util;

public class TimestampedLogSource : ILogSource
{
    public string SourceName { get; }

    public TimestampedLogSource(string sourceName) => SourceName = sourceName;

    public event EventHandler<LogEventArgs> LogEvent;

    // ReSharper disable once MemberCanBePrivate.Global
    public void Log(LogLevel level, object data, string context = null)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var builder = new StringBuilder($"[{timestamp}]");

        if (context != null)
        {
            builder.Append($"[{context}]");
        }

        builder.Append(" ");
        builder.Append(data);

        LogEvent?.Invoke(this, new LogEventArgs(builder.ToString(), level, this));
    }

    public void LogFatal(object data, string context = null) => Log(LogLevel.Fatal, data, context);
    public void LogError(object data, string context = null) => Log(LogLevel.Error, data, context);
    public void LogWarning(object data, string context = null) => Log(LogLevel.Warning, data, context);
    public void LogMessage(object data, string context = null) => Log(LogLevel.Message, data, context);
    public void LogInfo(object data, string context = null) => Log(LogLevel.Info, data, context);
    public void LogDebug(object data, string context = null) => Log(LogLevel.Debug, data, context);

    public void Dispose()
    {
    }
}