# Soenneker.Maui.Blazor.BrowserLogger
[![](https://img.shields.io/nuget/v/soenneker.maui.blazor.browserlogger.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maui.blazor.browserlogger/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maui.blazor.browserlogger/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.maui.blazor.browserlogger/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.maui.blazor.browserlogger.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maui.blazor.browserlogger/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maui.blazor.browserlogger/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.maui.blazor.browserlogger/actions/workflows/codeql.yml)

Writes `Microsoft.Extensions.Logging` messages from a .NET MAUI Blazor app to the browser developer console.

## Installation

```bash
dotnet add package Soenneker.Maui.Blazor.BrowserLogger
```

## Registration

Add the provider in `MauiProgram.cs`:

```csharp
using Soenneker.Maui.Blazor.BrowserLogger.Extensions;

builder.Logging.AddMauiBlazorBrowser();
```

The provider can receive log entries immediately, but JavaScript output begins after it has an `IJSRuntime`. Initialize it once from a component that lives for the duration of the `BlazorWebView`, such as `MainLayout.razor`:

```razor
@inject IJSRuntime JSRuntime
@inject IMauiBlazorJsInteropLoggingService BrowserLogging

@code {
    protected override void OnInitialized()
    {
        BrowserLogging.Initialize(JSRuntime);
    }
}
```

Add these namespaces to the component or `_Imports.razor`:

```razor
@using Microsoft.JSInterop
@using Soenneker.Maui.Blazor.BrowserLogger.Abstract
```

## Usage

Use the standard `ILogger<T>` API anywhere in the app:

```razor
@inject ILogger<Orders> Logger

@code {
    private void OrderFailed(Exception exception)
    {
        Logger.LogError(exception, "Could not load the order");
    }
}
```

Trace and debug messages use `console.debug`, information uses `console.info`, warnings use `console.warn`, and errors and critical messages use `console.error`. Configure category and minimum-level filtering through the normal .NET logging configuration.

Messages are delivered asynchronously and may be lost during shutdown or when the browser cannot keep up. Before initialization, the service retains only the newest buffered messages.
