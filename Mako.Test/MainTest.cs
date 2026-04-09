using System.Threading.Tasks;
using Mako.Global.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Test;

[TestClass]
public sealed class MainTest
{
    [TestMethod]
    [DoNotParallelize]
    [DataRow("<REFRESH_TOKEN>")]
    public async Task TestRefreshToken(string refreshToken)
    {
        TestSettings.Client.SetToken(refreshToken);

        var tags = await TestSettings.Client.GetTrendingTagsAsync(TargetFilter.ForIos);

        Assert.IsGreaterThan(0, tags.Count);
    }
}
