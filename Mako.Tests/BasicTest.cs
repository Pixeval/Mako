// Copyright (c) Mako.Tests.
// Licensed under the GPL-3.0 License.

using System.Threading.Tasks;
using Mako.Global.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mako.Tests;

[TestClass]
public sealed class BasicTest
{
    [TestMethod]
    [DataRow("<REFRESH_TOKEN>")]
    public async Task TestRefreshTokenValidAsync(string refreshToken)
    {
        TestSettings.Client.SetToken(refreshToken);

        var result = await TestSettings.Client.IdentifyTokenAsync();

        Assert.IsTrue(result);

        var tags = await TestSettings.Client.GetWorkTrendingTagsAsync(SimpleWorkType.Illustration);

        Assert.IsNotEmpty(tags);
    }
}
