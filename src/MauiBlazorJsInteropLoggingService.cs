using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Maui.Blazor.BrowserLogger.Abstract;

namespace Soenneker.Maui.Blazor.BrowserLogger;

///<inheritdoc cref="IMauiBlazorJsInteropLoggingService"/>
public sealed class MauiBlazorJsInteropLoggingService : IMauiBlazorJsInteropLoggingService
{
    private IJSRuntime? _jsRuntime;
    private readonly ConcurrentQueue<(string logMethod, string message)> _logQueue = new();
    private PeriodicTimer? _logTimer;
    private CancellationTokenSource? _cts;
    private bool _isProcessingLogs;

    private bool _initialized;

    public void Initialize(IJSRuntime jsRuntime, CancellationToken cancellationToken = default)
    {
        if (_initialized)
            return;

        _initialized = true;

        _jsRuntime = jsRuntime;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

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
            while (await _logTimer!.WaitForNextTickAsync(_cts!.Token).NoSync())
            {
                if (_jsRuntime != null && !_isProcessingLogs)
                {
                    _isProcessingLogs = true;

                    while (_logQueue.TryDequeue(out (string logMethod, string message) log))
                    {
                        await _jsRuntime.InvokeVoidAsync(log.logMethod, _cts.Token, log.message).NoSync();
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
        _logTimer?.Dispose();

        if (_cts != null)
        {
            await _cts.CancelAsync().NoSync();
            _cts.Dispose();
        }
    }
}