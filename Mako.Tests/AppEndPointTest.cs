// Copyright (c) Mako.Tests.
// Licensed under the GPL-3.0 License.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Tests;

[TestClass]
public sealed class AppEndPointTest
{
    [TestMethod]
    [DataRow(144303976)]
    public async Task TestGetIllustrationAsync(long id)
    {
        var result = await TestSettings.Client.GetIllustrationFromIdAsync(id);
        Assert.AreEqual(id, result.Id);
    }

    [TestMethod]
    [DataRow(21793115)]
    public async Task TestGetNovelAsync(long id)
    {
        var result = await TestSettings.Client.GetNovelFromIdAsync(id);
        Assert.AreEqual(id, result.Id);
    }

    [TestMethod]
    [DataRow(21793115)]
    public async Task TestGetNovelContentAsync(long id)
    {
        var result = await TestSettings.Client.GetNovelContentAsync(id);
        Assert.AreEqual(id, result.Id);
    }

    [TestMethod]
    [DataRow(142475530)]
    public async Task TestGetUgoiraMetadataAsync(long id)
    {
        var result = await TestSettings.Client.GetUgoiraMetadataAsync(id);
        Assert.IsNotEmpty(result.Frames);
    }

    [TestMethod]
    [DataRow(142475530)]
    public async Task TestGetSearchOptionsAsync(long id)
    {
        var result = await TestSettings.Client.GetSearchOptionsAsync();
        Assert.IsNotEmpty(result.IllustrationOptions.BookmarkRanges);
    }
}
