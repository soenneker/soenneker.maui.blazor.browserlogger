using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;

namespace Soenneker.Maui.Blazor.BrowserLogger.Extensions;

/// <summary>
/// Represents the maui blazor browser logging extension.
/// </summary>
public static class MauiBlazorBrowserLoggingExtension
{
    /// <summary>
    /// Adds maui blazor browser.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The result of the operation.</returns>
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