using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;

namespace Soenneker.Maui.Blazor.BrowserLogger.Extensions;

public static class MauiBlazorBrowserLoggingExtension
{
    public static ILoggingBuilder AddMauiBlazorBrowser(this ILoggingBuilder builder)
    {
        builder.Services.TryAddSingleton<IMauiBlazorJsInteropLoggingService, MauiBlazorJsInteropLoggingService>();

        builder.Services.TryAddSingleton<ILoggerProvider>(sp =>
        {
            var jsInteropService = sp.GetRequiredService<IMauiBlazorJsInteropLoggingService>();
            return new MauiBlazorBrowserLoggerProvider(jsInteropService);
        });

        return builder;
    }
}