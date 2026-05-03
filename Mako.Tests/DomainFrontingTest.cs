using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Tests;

[TestClass]
public sealed class DomainFrontingTest
{
    [TestMethod]
    [DataRow(DomainFrontingType.Fragmentation)]
    [DataRow(DomainFrontingType.Ech)]
    [DataRow(DomainFrontingType.Desync)]
    public async Task TestDomainFronting(DomainFrontingType domainFrontingType)
    {
        TestSettings.Client.Configuration.DomainFronting = true;
        TestSettings.Client.Configuration.DomainFrontingType = domainFrontingType;

        var tags = await TestSettings.Client.GetTrendingTagsAsync();

        Assert.IsNotEmpty(tags);
    }

    [TestMethod]
    public async Task TestDomainFrontingDownloadImage()
    {
        TestSettings.Client.Configuration.DomainFronting = true;

        var imageClient = TestSettings.Client.GetImageDownloadClient();

        var result = await imageClient.GetStreamAsync("https://s.pximg.net/common/images/no_profile_s.png");

        Assert.IsNotNull(result);
    }
}
