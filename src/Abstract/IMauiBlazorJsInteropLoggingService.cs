using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Soenneker.Maui.Blazor.BrowserLogger.Abstract;
public interface IMauiBlazorJsInteropLoggingService : IAsyncDisposable
{
    /// <summary>
    /// Initializes the JavaScript runtime for interop operations.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime instance.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Initialize(IJSRuntime jsRuntime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queues a log message to be processed and sent to the JavaScript runtime.
    /// </summary>
    /// <param name="logMethod">The JavaScript logging method to invoke.</param>
    /// <param name="message">The message to log.</param>
    void QueueLog(string logMethod, string message);
}