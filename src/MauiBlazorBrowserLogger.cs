using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using System;

namespace Soenneker.Maui.Blazor.BrowserLogger;

/// <inheritdoc cref="IMauiBlazorBrowserLogger"/>
public class MauiBlazorBrowserLogger : IMauiBlazorBrowserLogger
{
    private readonly IMauiBlazorJsInteropLoggingService _jsInteropService;
    private readonly string _categoryName;

    public MauiBlazorBrowserLogger(IMauiBlazorJsInteropLoggingService jsInteropService, string categoryName)
    {
        _jsInteropService = jsInteropService;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string message = formatter(state, exception);
        string logLevelString = GetConsoleMethod(logLevel);

        // Send log message to the UI service (where it will be processed safely)
        _jsInteropService.QueueLog(logLevelString, $"[{logLevelString}] {_categoryName}: {message}");
    }

    private static string GetConsoleMethod(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => "console.debug",
        LogLevel.Debug => "console.debug",
        LogLevel.Information => "console.info",
        LogLevel.Warning => "console.warn",
        LogLevel.Error => "console.error",
        LogLevel.Critical => "console.error",
        _ => "console.log"
    };
}