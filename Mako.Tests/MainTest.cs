using System.Threading.Tasks;
using Mako.Global.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Test;

[TestClass]
public sealed class MainTest
{
    private const string RefreshToken = "<REFRESH_TOKEN>";

    [TestMethod]
    [DoNotParallelize]
    [DataRow(RefreshToken)]
    public async Task TestRefreshToken(string refreshToken)
    {
        TestSettings.Client.SetToken(refreshToken);

        var tags = await TestSettings.Client.GetTrendingTagsAsync(TargetFilter.ForIos);
        
        Assert.IsGreaterThan(0, tags.Count);
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow(DomainFrontingType.Fragmentation)]
    [DataRow(DomainFrontingType.Ech)]
    [DataRow(DomainFrontingType.Desync)]
    public async Task TestDomainFronting(DomainFrontingType domainFrontingType)
    {
        TestSettings.Client.SetToken(RefreshToken);
        TestSettings.Client.Configuration.DomainFronting = true;
        TestSettings.Client.Configuration.DomainFrontingType = domainFrontingType;

        var tags = await TestSettings.Client.GetTrendingTagsAsync(TargetFilter.ForIos);

        Assert.IsGreaterThan(0, tags.Count);
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task TestDomainFrontingDownloadImage()
    {
        TestSettings.Client.SetToken(RefreshToken);
        TestSettings.Client.Configuration.DomainFronting = true;

        var imageClient = TestSettings.Client.GetImageDownloadClient();

        var result = await imageClient.GetStreamAsync("https://s.pximg.net/common/images/no_profile_s.png");

        Assert.IsNotNull(result);
    }
}
