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

public class PageCloseTests : PageTestEx
{
    [PlaywrightTest("page-close.spec.ts", "should close page")]
    public async Task ShouldClosePage()
    {
        var newPage = await Context.NewPageAsync();
        Assert.False(newPage.IsClosed);
        await newPage.CloseAsync();
        Assert.True(newPage.IsClosed);
    }

    [PlaywrightTest("page-close.spec.ts", "should fire close event")]
    public async Task ShouldFireCloseEvent()
    {
        var newPage = await Context.NewPageAsync();
        var closeEventFired = false;
        newPage.Close += (_, _) => closeEventFired = true;
        await newPage.CloseAsync();
        Assert.True(closeEventFired);
    }

    [PlaywrightTest("page-close.spec.ts", "should reject all promises when page is closed")]
    public async Task ShouldRejectAllPromisesWhenPageIsClosed()
    {
        var newPage = await Context.NewPageAsync();
        var exception = await PlaywrightAssert.ThrowsAsync<PlaywrightException>(() => TaskUtils.WhenAll(
            newPage.EvaluateAsync<string>("() => new Promise(() => {})"),
            newPage.CloseAsync()
        ));
        StringAssert.Contains(TestConstants.TargetClosedErrorMessage, exception.Message);
    }

    [PlaywrightTest("page-close.spec.ts", "should not be visible in context.pages after close")]
    public async Task ShouldNotBeVisibleInContextPagesAfterClose()
    {
        var newPage = await Context.NewPageAsync();
        CollectionAssert.Contains(Context.Pages, newPage);
        await newPage.CloseAsync();
        CollectionAssert.DoesNotContain(Context.Pages, newPage);
    }

    [PlaywrightTest("page-close.spec.ts", "should run beforeunload if asked for")]
    public async Task ShouldRunBeforeUnloadIfAskedFor()
    {
        await Page.GotoAsync(Server.Prefix + "/beforeunload.html");
        var dialogTask = new TaskCompletionSource<IDialog>();
        Page.Dialog += (_, dialog) => dialogTask.SetResult(dialog);

        var pageClosingTask = Page.CloseAsync(new() { RunBeforeUnload = true });
        var dialog = await dialogTask.Task;
        Assert.AreEqual("beforeunload", dialog.Type);
        Assert.AreEqual(string.Empty, dialog.DefaultValue);
        Assert.AreEqual("Are you sure you want to leave?", dialog.Message);

        await dialog.AcceptAsync();
        await pageClosingTask;
    }

    [PlaywrightTest("page-close.spec.ts", "should not run beforeunload by default")]
    public async Task ShouldNotRunBeforeUnloadByDefault()
    {
        await Page.GotoAsync(Server.Prefix + "/beforeunload.html");
        var dialogFired = false;
        Page.Dialog += (_, _) => dialogFired = true;
        await Page.CloseAsync();
        Assert.False(dialogFired);
    }

    [PlaywrightTest("page-close.spec.ts", "should be able to close page twice")]
    public async Task ShouldBeAbleToClosePageTwice()
    {
        var newPage = await Context.NewPageAsync();
        await newPage.CloseAsync();
        await newPage.CloseAsync();
        Assert.True(newPage.IsClosed);
    }

    [PlaywrightTest("page-close.spec.ts", "should terminate background tasks")]
    public async Task ShouldTerminateBackgroundTasks()
    {
        var newPage = await Context.NewPageAsync();
        await newPage.GotoAsync(Server.EmptyPage);
        await newPage.EvaluateAsync("() => { window['intervalId'] = setInterval(() => console.log('tick'), 1000); }");
        await newPage.CloseAsync();
        Assert.True(newPage.IsClosed);
    }
}
