using Soenneker.Maui.Blazor.BrowserLogger.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Maui.Blazor.BrowserLogger.Tests;

[Collection("Collection")]
public class MauiBlazorBrowserLoggerTests : FixturedUnitTest
{
    private readonly IMauiBlazorBrowserLogger _util;

    public MauiBlazorBrowserLoggerTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IMauiBlazorBrowserLogger>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
