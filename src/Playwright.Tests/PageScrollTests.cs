/*
 * MIT License
 *
 * Copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Microsoft.Playwright.Tests;

public class PageScrollTests : PageTestEx
{
    [PlaywrightTest("page-scroll.spec.ts", "should scroll to the bottom of the page")]
    public async Task ShouldScrollToTheBottomOfThePage()
    {
        await Page.SetContentAsync(@"
            <div style='height: 5000px;'>
                <div style='position: absolute; top: 4500px;' id='target'>Target</div>
            </div>
        ");
        await Page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");
        var scrollY = await Page.EvaluateAsync<int>("() => window.scrollY");
        Assert.Greater(scrollY, 4000);
    }

    [PlaywrightTest("page-scroll.spec.ts", "should scroll element into view")]
    public async Task ShouldScrollElementIntoView()
    {
        await Page.SetContentAsync(@"
            <div style='height: 5000px;'>
                <div style='position: absolute; top: 4500px;' id='target'>Target</div>
            </div>
        ");
        var element = await Page.QuerySelectorAsync("#target");
        await element.ScrollIntoViewIfNeededAsync();
        var boundingBox = await element.BoundingBoxAsync();
        Assert.NotNull(boundingBox);
    }

    [PlaywrightTest("page-scroll.spec.ts", "should be able to get scroll position")]
    public async Task ShouldBeAbleToGetScrollPosition()
    {
        await Page.SetContentAsync(@"
            <div style='height: 5000px;'>
                <div style='position: absolute; top: 2500px;' id='target'>Target</div>
            </div>
        ");
        await Page.EvaluateAsync("() => window.scrollTo(0, 2000)");
        var scrollY = await Page.EvaluateAsync<int>("() => window.scrollY");
        Assert.AreEqual(2000, scrollY);
    }

    [PlaywrightTest("page-scroll.spec.ts", "should scroll with mouse wheel")]
    public async Task ShouldScrollWithMouseWheel()
    {
        await Page.SetContentAsync(@"
            <div style='height: 5000px; width: 5000px;'>
                <div id='target'>Target</div>
            </div>
        ");
        await Page.Mouse.WheelAsync(0, 1000);
        var scrollY = await Page.EvaluateAsync<int>("() => window.scrollY");
        Assert.Greater(scrollY, 0);
    }

    [PlaywrightTest("page-scroll.spec.ts", "should scroll horizontally")]
    public async Task ShouldScrollHorizontally()
    {
        await Page.SetContentAsync(@"
            <div style='height: 1000px; width: 5000px;'>
                <div style='position: absolute; left: 4500px;' id='target'>Target</div>
            </div>
        ");
        await Page.EvaluateAsync("() => window.scrollTo(2000, 0)");
        var scrollX = await Page.EvaluateAsync<int>("() => window.scrollX");
        Assert.AreEqual(2000, scrollX);
    }

    [PlaywrightTest("page-scroll.spec.ts", "should maintain scroll position on navigation")]
    public async Task ShouldMaintainScrollPositionOnNavigation()
    {
        await Page.GotoAsync(Server.Prefix + "/grid.html");
        await Page.EvaluateAsync("() => window.scrollTo(0, 500)");
        var scrollYBefore = await Page.EvaluateAsync<int>("() => window.scrollY");
        await Page.EvaluateAsync("() => window.history.replaceState({}, '', window.location.href)");
        var scrollYAfter = await Page.EvaluateAsync<int>("() => window.scrollY");
        Assert.AreEqual(scrollYBefore, scrollYAfter);
    }

    [PlaywrightTest("page-scroll.spec.ts", "should handle overflow scroll")]
    public async Task ShouldHandleOverflowScroll()
    {
        await Page.SetContentAsync(@"
            <div id='scrollable' style='height: 200px; width: 200px; overflow: scroll;'>
                <div style='height: 1000px; width: 1000px;'>
                    <div style='position: absolute; top: 800px; left: 800px;' id='target'>Target</div>
                </div>
            </div>
        ");
        await Page.EvaluateAsync("() => document.getElementById('scrollable').scrollTo(500, 500)");
        var scrollTop = await Page.EvaluateAsync<int>("() => document.getElementById('scrollable').scrollTop");
        Assert.AreEqual(500, scrollTop);
    }
}
