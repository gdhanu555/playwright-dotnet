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

using System.Text.Json;

namespace Microsoft.Playwright.Tests;

public class JSHandleJsonValueTests : PageTestEx
{
    [PlaywrightTest("jshandle-json-value.spec.ts", "should work")]
    public async Task ShouldWork()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => ({ foo: 'bar'})");
        var json = await aHandle.JsonValueAsync<JsonElement>();
        Assert.AreEqual("bar", json.GetProperty("foo").GetString());
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with dates")]
    public async Task ShouldWorkWithDates()
    {
        var dateHandle = await Page.EvaluateHandleAsync("() => new Date('2017-09-26T00:00:00.000Z')");
        var json = await dateHandle.JsonValueAsync<DateTime>();
        Assert.AreEqual(2017, json.Year);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should handle circular objects")]
    public async Task ShouldHandleCircularObjects()
    {
        var windowHandle = await Page.EvaluateHandleAsync("const a = {}; a.b = a; a");
        var a = await windowHandle.JsonValueAsync<RecursiveCircularObjectClass>();
        Assert.AreEqual(a, a.b);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with arrays")]
    public async Task ShouldWorkWithArrays()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => [1, 2, 3, 4, 5]");
        var json = await aHandle.JsonValueAsync<int[]>();
        Assert.AreEqual(5, json.Length);
        Assert.AreEqual(1, json[0]);
        Assert.AreEqual(5, json[4]);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with nested objects")]
    public async Task ShouldWorkWithNestedObjects()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => ({ outer: { inner: { value: 42 } } })");
        var json = await aHandle.JsonValueAsync<JsonElement>();
        Assert.AreEqual(42, json.GetProperty("outer").GetProperty("inner").GetProperty("value").GetInt32());
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with numbers")]
    public async Task ShouldWorkWithNumbers()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => 123");
        var json = await aHandle.JsonValueAsync<int>();
        Assert.AreEqual(123, json);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with strings")]
    public async Task ShouldWorkWithStrings()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => 'hello world'");
        var json = await aHandle.JsonValueAsync<string>();
        Assert.AreEqual("hello world", json);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with booleans")]
    public async Task ShouldWorkWithBooleans()
    {
        var trueHandle = await Page.EvaluateHandleAsync("() => true");
        var trueValue = await trueHandle.JsonValueAsync<bool>();
        Assert.True(trueValue);

        var falseHandle = await Page.EvaluateHandleAsync("() => false");
        var falseValue = await falseHandle.JsonValueAsync<bool>();
        Assert.False(falseValue);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with null")]
    public async Task ShouldWorkWithNull()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => null");
        var json = await aHandle.JsonValueAsync<string>();
        Assert.Null(json);
    }

    [PlaywrightTest("jshandle-json-value.spec.ts", "should work with mixed types in arrays")]
    public async Task ShouldWorkWithMixedTypesInArrays()
    {
        var aHandle = await Page.EvaluateHandleAsync("() => [1, 'two', true, null, { key: 'value' }]");
        var json = await aHandle.JsonValueAsync<JsonElement>();
        Assert.AreEqual(JsonValueKind.Array, json.ValueKind);
        Assert.AreEqual(5, json.GetArrayLength());
    }

    private class RecursiveCircularObjectClass
    {
        public RecursiveCircularObjectClass b { get; set; }
    }
}
