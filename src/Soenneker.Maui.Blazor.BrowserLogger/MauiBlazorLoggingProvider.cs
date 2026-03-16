using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using System.Collections.Concurrent;

namespace Soenneker.Maui.Blazor.BrowserLogger;

public sealed class MauiBlazorBrowserLoggerProvider : ILoggerProvider
{
    private readonly IMauiBlazorJsInteropLoggingService _jsInteropService;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public MauiBlazorBrowserLoggerProvider(IMauiBlazorJsInteropLoggingService jsInteropService)
    {
        _jsInteropService = jsInteropService;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, static (name, state) =>
            new MauiBlazorBrowserLogger(state, name), _jsInteropService);
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}