using Microsoft.JSInterop;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using Soenneker.Utils.CancellationScopes;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Soenneker.Maui.Blazor.BrowserLogger;

/// <inheritdoc cref="IMauiBlazorJsInteropLoggingService" />
public sealed class MauiBlazorJsInteropLoggingService : IMauiBlazorJsInteropLoggingService
{
    private readonly Channel<LogEntry> _channel = Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(1024)
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false,
        FullMode = BoundedChannelFullMode.DropOldest
    });

    private readonly CancellationScope _cancellationScope = new();

    private IJSRuntime? _jsRuntime;
    private CancellationTokenSource? _linkedSource;
    private CancellationToken _linkedToken;
    private Task? _processingTask;
    private int _initialized;

    public void Initialize(IJSRuntime jsRuntime, CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _initialized, 1) != 0)
            return;

        _jsRuntime = jsRuntime;

        _linkedToken = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        _linkedSource = source;

        _processingTask = ProcessLogsAsync();
    }

    public void QueueLog(string logMethod, string message)
    {
        _channel.Writer.TryWrite(new LogEntry(logMethod, message));
    }

    private async Task ProcessLogsAsync()
    {
        try
        {
            ChannelReader<LogEntry> reader = _channel.Reader;
            var arguments = new object?[1];

            while (await reader.WaitToReadAsync(_linkedToken).NoSync())
            {
                while (reader.TryRead(out LogEntry entry))
                {
                    IJSRuntime? jsRuntime = _jsRuntime;
                    if (jsRuntime is null)
                        continue;

                    arguments[0] = entry.Message;
                    try
                    {
                        await jsRuntime.InvokeVoidAsync(entry.LogMethod, _linkedToken, arguments).NoSync();
                    }
                    finally
                    {
                        arguments[0] = null;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
        catch (JSDisconnectedException)
        {
            // Browser/Blazor circuit shut down
        }
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();

        if (_linkedSource is not null)
            await _linkedSource.CancelAsync().NoSync();

        if (_processingTask is not null)
        {
            try
            {
                await _processingTask.NoSync();
            }
            catch (OperationCanceledException)
            {
            }
        }

        _linkedSource?.Dispose();
        _linkedSource = null;
        _processingTask = null;
        _jsRuntime = null;

        await _cancellationScope.DisposeAsync().NoSync();
    }

}
