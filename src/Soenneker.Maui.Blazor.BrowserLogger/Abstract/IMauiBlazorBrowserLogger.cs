using Microsoft.Extensions.Logging;
using System;

namespace Soenneker.Maui.Blazor.BrowserLogger.Abstract;

/// <summary>
/// Adds the ability to browser console log in Blazor MAUI apps
/// </summary>
public interface IMauiBlazorBrowserLogger : ILogger
{
    /// <summary>
    /// Begins a logging scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state object.</param>
    /// <returns>An IDisposable representing the scope.</returns>
    new IDisposable? BeginScope<TState>(TState state) where TState : notnull;

    /// <summary>
    /// Determines if the logger is enabled for the specified log level.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <returns>True if logging is enabled, otherwise false.</returns>
    new bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// Logs a message with the specified log level and event ID.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <param name="logLevel">The log level.</param>
    /// <param name="eventId">The event ID.</param>
    /// <param name="state">The state object.</param>
    /// <param name="exception">An optional exception.</param>
    /// <param name="formatter">A function to format the log message.</param>
    new void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter);
}