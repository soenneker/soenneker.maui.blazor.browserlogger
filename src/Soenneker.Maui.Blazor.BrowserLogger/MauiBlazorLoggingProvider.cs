using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using System.Collections.Concurrent;

namespace Soenneker.Maui.Blazor.BrowserLogger;

/// <summary>
/// Represents the maui blazor browser logger provider.
/// </summary>
public sealed class MauiBlazorBrowserLoggerProvider : ILoggerProvider
{
    private readonly IMauiBlazorJsInteropLoggingService _jsInteropService;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public MauiBlazorBrowserLoggerProvider(IMauiBlazorJsInteropLoggingService jsInteropService)
    {
        _jsInteropService = jsInteropService;
    }

    /// <summary>
    /// Creates logger.
    /// </summary>
    /// <param name="categoryName">The category name.</param>
    /// <returns>The result of the operation.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, static (name, state) =>
            new MauiBlazorBrowserLogger(state, name), _jsInteropService);
    }

    /// <summary>
    /// Releases resources used by the current instance.
    /// </summary>
    public void Dispose()
    {
        _loggers.Clear();
    }
}