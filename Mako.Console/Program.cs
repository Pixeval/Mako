using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Mako.Model;
using Mako.Util;

namespace Mako.Console
{
    public static class Program
    {
        private static readonly Session Session = "{\"access_token\":\"KTPLMlPIQuO7stIl4yI5Fka5A4o65gUo_iFFCuW2v6w\",\"expires_in\":3600,\"token_type\":\"bearer\",\"scope\":\"\",\"refresh_token\":\"5QOHeUpB-cFNjrXmqOU0YFv2QLnlnKHmJ4RJF4YSSQI\",\"user\":{\"profile_image_urls\":{\"px_16x16\":\"https:\\/\\/i.pximg.net\\/user-profile\\/img\\/2019\\/01\\/13\\/21\\/00\\/08\\/15255001_2f78dcb00cc01551c55586280352571a_16.jpg\",\"px_50x50\":\"https:\\/\\/i.pximg.net\\/user-profile\\/img\\/2019\\/01\\/13\\/21\\/00\\/08\\/15255001_2f78dcb00cc01551c55586280352571a_50.jpg\",\"px_170x170\":\"https:\\/\\/i.pximg.net\\/user-profile\\/img\\/2019\\/01\\/13\\/21\\/00\\/08\\/15255001_2f78dcb00cc01551c55586280352571a_170.jpg\"},\"id\":\"17861677\",\"name\":\"December0730\",\"account\":\"2653221698\",\"mail_address\":\"2653221698@qq.com\",\"is_premium\":false,\"x_restrict\":2,\"is_mail_authorized\":true,\"require_policy_agreement\":true},\"response\":{\"access_token\":\"KTPLMlPIQuO7stIl4yI5Fka5A4o65gUo_iFFCuW2v6w\",\"expires_in\":3600,\"token_type\":\"bearer\",\"scope\":\"\",\"refresh_token\":\"5QOHeUpB-cFNjrXmqOU0YFv2QLnlnKHmJ4RJF4YSSQI\",\"user\":{\"profile_image_urls\":{\"px_16x16\":\"https:\\/\\/i.pximg.net\\/user-profile\\/img\\/2019\\/01\\/13\\/21\\/00\\/08\\/15255001_2f78dcb00cc01551c55586280352571a_16.jpg\",\"px_50x50\":\"https:\\/\\/i.pximg.net\\/user-profile\\/img\\/2019\\/01\\/13\\/21\\/00\\/08\\/15255001_2f78dcb00cc01551c55586280352571a_50.jpg\",\"px_170x170\":\"https:\\/\\/i.pximg.net\\/user-profile\\/img\\/2019\\/01\\/13\\/21\\/00\\/08\\/15255001_2f78dcb00cc01551c55586280352571a_170.jpg\"},\"id\":\"17861677\",\"name\":\"December0730\",\"account\":\"2653221698\",\"mail_address\":\"2653221698@qq.com\",\"is_premium\":false,\"x_restrict\":2,\"is_mail_authorized\":true,\"require_policy_agreement\":true}}}"
            .FromJson<TokenResponse>(option => option.IgnoreNullValues = true)!
            .ToSession("123456")
            .Apply(s => s.Bypass = true);

        private static readonly MakoClient MakoClient = new(Session, CultureInfo.CurrentCulture);

        public static async Task Main(string[] args)
        {
            var bookmarks = MakoClient.Bookmarks("333556", PrivacyPolicy.Public);
            await foreach (var i in bookmarks)
            {
                if (i != null)
                {
                    System.Console.WriteLine(i.Id);
                }
            }
        }
    }
}