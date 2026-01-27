using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using Soenneker.Utils.CancellationScopes;

namespace Soenneker.Maui.Blazor.BrowserLogger;

///<inheritdoc cref="IMauiBlazorJsInteropLoggingService"/>
public sealed class MauiBlazorJsInteropLoggingService : IMauiBlazorJsInteropLoggingService
{
    private IJSRuntime? _jsRuntime;
    private readonly ConcurrentQueue<(string logMethod, string message)> _logQueue = new();
    private PeriodicTimer? _logTimer;
    private readonly CancellationScope _cancellationScope = new();
    private CancellationTokenSource? _linkedSource;
    private CancellationToken _linkedToken;
    private bool _isProcessingLogs;

    private bool _initialized;

    public void Initialize(IJSRuntime jsRuntime, CancellationToken cancellationToken = default)
    {
        if (_initialized)
            return;

        _initialized = true;

        _jsRuntime = jsRuntime;

        _linkedToken = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);
        _linkedSource = source;

        _logTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        _ = StartLogProcessing();
    }

    public void QueueLog(string logMethod, string message)
    {
        _logQueue.Enqueue((logMethod, message));
    }

    private async ValueTask StartLogProcessing()
    {
        try
        {
            while (await _logTimer!.WaitForNextTickAsync(_linkedToken).NoSync())
            {
                if (_jsRuntime != null && !_isProcessingLogs)
                {
                    _isProcessingLogs = true;

                    while (_logQueue.TryDequeue(out (string logMethod, string message) log))
                    {
                        await _jsRuntime.InvokeVoidAsync(log.logMethod, _linkedToken, log.message).NoSync();
                    }

                    _isProcessingLogs = false;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_linkedSource != null)
        {
            await _linkedSource.CancelAsync().NoSync();
        }

        _logTimer?.Dispose();

        if (_linkedSource != null)
        {
            _linkedSource.Dispose();
            _linkedSource = null;
        }

        _logTimer = null;
        _jsRuntime = null;

        await _cancellationScope.DisposeAsync().NoSync();
    }
}