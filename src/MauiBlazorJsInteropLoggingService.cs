using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
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

    public async ValueTask Initialize(IJSRuntime jsRuntime, CancellationToken cancellationToken = default)
    {
        _jsRuntime = jsRuntime;

        // Start processing logs only once
        if (_logTimer == null)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _logTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

            await StartLogProcessing();
        }
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
            await _cts.CancelAsync();
            _cts.Dispose();
        }
    }
}