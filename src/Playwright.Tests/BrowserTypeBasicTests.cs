/*
 * MIT License
 *
 * Copyright (c) 2020 Darío Kondratiuk
 * Modifications copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
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

public class BrowserTypeBasicTests : PlaywrightTestEx
{
    [PlaywrightTest("browsertype-basic.spec.ts", "browserType.executablePath should work")]
    public void BrowserTypeExecutablePathShouldWork() => Assert.True(File.Exists(BrowserType.ExecutablePath));

    [PlaywrightTest("browsertype-basic.spec.ts", "browserType.name should work")]
    public void BrowserTypeNameShouldWork()
        => Assert.AreEqual(
            TestConstants.BrowserName switch
            {
                "webkit" => "webkit",
                "firefox" => "firefox",
                "chromium" => "chromium",
                _ => null
            },
            BrowserType.Name);

    [PlaywrightTest("browsertype-basic.spec.ts", "browserType.executablePath should be a valid path")]
    public void BrowserTypeExecutablePathShouldBeValidPath()
    {
        var path = BrowserType.ExecutablePath;
        Assert.IsNotEmpty(path);
        Assert.True(Path.IsPathRooted(path));
    }

    [PlaywrightTest("browsertype-basic.spec.ts", "should return correct browser name for each browser type")]
    public void ShouldReturnCorrectBrowserNameForEachBrowserType()
    {
        var name = BrowserType.Name;
        Assert.That(name, Is.EqualTo("chromium").Or.EqualTo("firefox").Or.EqualTo("webkit"));
    }

    [PlaywrightTest("browsertype-basic.spec.ts", "should have version property")]
    public async Task ShouldHaveVersionProperty()
    {
        var browser = await BrowserType.LaunchAsync();
        Assert.IsNotEmpty(browser.Version);
        await browser.CloseAsync();
    }

    [PlaywrightTest("browsertype-basic.spec.ts", "browser version should contain numbers")]
    public async Task BrowserVersionShouldContainNumbers()
    {
        var browser = await BrowserType.LaunchAsync();
        var version = browser.Version;
        Assert.IsTrue(version.Any(char.IsDigit), $"Browser version '{version}' should contain numbers");
        await browser.CloseAsync();
    }

    [PlaywrightTest("browsertype-basic.spec.ts", "should be able to launch multiple browsers")]
    public async Task ShouldBeAbleToLaunchMultipleBrowsers()
    {
        var browser1 = await BrowserType.LaunchAsync();
        var browser2 = await BrowserType.LaunchAsync();
        Assert.AreNotEqual(browser1, browser2);
        await browser1.CloseAsync();
        await browser2.CloseAsync();
    }

    [PlaywrightTest("browsertype-basic.spec.ts", "should be able to check if browser is connected")]
    public async Task ShouldBeAbleToCheckIfBrowserIsConnected()
    {
        var browser = await BrowserType.LaunchAsync();
        Assert.True(browser.IsConnected);
        await browser.CloseAsync();
        Assert.False(browser.IsConnected);
    }
}
