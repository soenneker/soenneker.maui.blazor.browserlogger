using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;

namespace Soenneker.Maui.Blazor.BrowserLogger;

public class MauiBlazorBrowserLoggerProvider : ILoggerProvider
{
    private readonly IMauiBlazorJsInteropLoggingService _jsInteropService;

    public MauiBlazorBrowserLoggerProvider(IMauiBlazorJsInteropLoggingService jsInteropService)
    {
        _jsInteropService = jsInteropService;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new MauiBlazorBrowserLogger(_jsInteropService, categoryName);
    }

    public void Dispose() { }
}