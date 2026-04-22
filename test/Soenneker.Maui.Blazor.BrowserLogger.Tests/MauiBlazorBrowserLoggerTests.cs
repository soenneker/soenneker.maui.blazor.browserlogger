using Soenneker.Tests.HostedUnit;

namespace Soenneker.Maui.Blazor.BrowserLogger.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class MauiBlazorBrowserLoggerTests : HostedUnitTest
{
    public MauiBlazorBrowserLoggerTests(Host host) : base(host)
    {

    }

    [Test]
    public void Default()
    {

    }
}
