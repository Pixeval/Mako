// Copyright (c) Mako.Tests.
// Licensed under the GPL-3.0 License.

using System;
using System.Threading.Tasks;
using Mako.Engine.Implements;
using Mako.Global.Enum;
using Mako.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Tests;

[TestClass]
public sealed class EnginesTest
{
    [TestMethod]
    [DataRow(WorkType.Illustration)]
    [DataRow(WorkType.Manga)]
    [DataRow(WorkType.Novel)]
    public async Task TestRecommendedWorksEngineAsync(WorkType type)
    {
        var engines = TestSettings.Client.RecommendedWorks(type);
        await foreach (var work in engines)
        {
            Assert.IsInstanceOfType(work, type switch
            {
                WorkType.Illustration => typeof(Illustration),
                WorkType.Manga => typeof(Illustration),
                WorkType.Novel => typeof(Novel),
                _ => throw new ArgumentOutOfRangeException()
            });
            Assert.IsGreaterThan(0, work.Id);
            return;
        }
        Assert.Fail("No works found.");
    }

    [TestMethod]
    public async Task TestSearchIllustrationsEngineAsync()
    {
        var engines = TestSettings.Client.SearchIllustrations(
            "女",
            SearchIllustrationTagMatchOption.PartialMatchForTags,
            WorkSortOption.PublishDateDescending,
            null,
            null,
            true,
            SearchIllustrationContentType.IllustrationAndMangaAndUgoira,
            SearchIllustrationRatioPattern.All,
            widthMin: 49,
            widthMax: 49,
            heightMin: null,
            heightMax: null,
            true,
            true,
            false);
        var count = 0;
        await foreach (var illustration in engines)
        {
            var width = illustration.Width;
            var height = illustration.Height;
            ++count;
            if (count is 20)
                return;
        }
        Assert.Fail("No works found.");
    }

    [TestMethod]
    public async Task TestSearchNovelsEngineAsync()
    {
        var engines = TestSettings.Client.SearchNovels(
            "女",
            SearchNovelTagMatchOption.Text,
            WorkSortOption.PublishDateDescending,
            null,
            null,
            true,
            null,
            SearchNovelContentLengthOption.None,
            null,
            null,
            false,
            null,
            false,
            true,
            true,
            false);
        var count = 0;
        await foreach (var novel in engines)
        {
            var textLength = novel.TextLength;
            ++count;
            if (count is 20)
                return;
        }
        Assert.Fail("No works found.");
    }

    [TestMethod]
    public async Task TestNewIllustrationsEngineAsync()
    {
        var engines = TestSettings.Client.NewIllustrations(false, 99999);
        var count = 0;
        await foreach (var illustration in engines)
        {
            ++count;
            if (count is 20)
                return;
        }
        Assert.Fail("No works found.");
    }
}
