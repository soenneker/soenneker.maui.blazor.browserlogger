using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using System;

namespace Soenneker.Maui.Blazor.BrowserLogger;

/// <inheritdoc cref="IMauiBlazorBrowserLogger"/>
public sealed class MauiBlazorBrowserLogger : IMauiBlazorBrowserLogger
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

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (formatter is null)
            throw new ArgumentNullException(nameof(formatter));

        string method = GetConsoleMethod(logLevel);
        string formatted = formatter(state, exception);

        string message = exception is null
            ? $"[{method}] {_categoryName}: {formatted}"
            : $"[{method}] {_categoryName}: {formatted}{Environment.NewLine}{exception}";

        _jsInteropService.QueueLog(method, message);
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