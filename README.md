[![](https://img.shields.io/nuget/v/soenneker.maui.blazor.browserlogger.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maui.blazor.browserlogger/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maui.blazor.browserlogger/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.maui.blazor.browserlogger/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.maui.blazor.browserlogger.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maui.blazor.browserlogger/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Maui.Blazor.BrowserLogger  
### 🌐 Blazor MAUI Console Logger – Log to the Browser Console Effortlessly

---

## 🚀 **What is this?**  
A **custom logger for .NET MAUI Blazor** that enables logging to the **browser console** using `IJSRuntime`, ensuring logs execute properly on the **UI thread**. It includes **background logging with a periodic timer**, ensuring logs are processed even when no UI event occurs.

---

## 📢 **Why Use This?**  
✅ **Standard MAUI loggers don’t work for browser console output**  
✅ **Blazor’s `IJSRuntime` must execute on the UI thread**  
✅ **.NET loggers process logs on a background thread, causing issues**  
✅ **Works seamlessly with any `ILogger` usage across your app**  

---

## ⚡ **Features**  
✔ **Blazor-compatible** – Ensures `IJSRuntime` runs on the UI thread.  
✔ **Automatic Logging** – Uses a `PeriodicTimer` to process logs continuously.  
✔ **Easy Integration** – Fully supports Blazor’s dependency injection system.  

---

## 📌 **Installation**  
Install via NuGet:  
```sh
dotnet add package BlazorMauiConsoleLogger
```

---

## 🔧 **Setup & Usage**  

### **1️⃣ Register the Logger in `MauiProgram.cs`**
Add the logger to the dependency injection container:  
```csharp
builder.Logging.AddMauiBlazorBrowser();
```

---

### **2️⃣ Initialize in `MainLayout.razor`**
Inject `IJSRuntime` and `IMauiBlazorJsInteropLoggingService` in a persistent layout or page:

```razor
@inject IJSRuntime JsRuntime
@inject IMauiBlazorJsInteropLoggingService LoggingService

@code {
    protected override async Task OnInitializedAsync()
    {
        await LoggingService.Initialize(JsRuntime);
    }
}
```

---

### **3️⃣ Inject & Use the Logger in a Component**
```razor
@inject ILogger<MyComponent> Logger

@code {
    protected override void OnInitialized()
    {
        Logger.LogInformation("Hello from Blazor Maui Console Logger!");
    }
}
```

📌 **Log output in the browser console:**
```plaintext
[Information] MyComponent: Hello from Blazor Maui Console Logger!
```

---