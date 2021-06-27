using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mako.Model;
using Mako.Util;

namespace Mako.Console
{
    /// <summary>
    /// 由于登录原因测试没法自动化，干脆这样好了
    /// </summary>
    [PublicAPI]
    public static class Program
    {
        private static readonly Session Session = "{\"Name\":\"December0730\",\"ExpireIn\":\"2021-06-28T04:48:35.9526347+08:00\",\"ConnectionTimeout\":5000,\"AccessToken\":\"fMh7H7Yg0MUccgDr5bsFVb4_sBOGTQZoegkocvOyqdo\",\"RefreshToken\":\"5QOHeUpB-cFNjrXmqOU0YFv2QLnlnKHmJ4RJF4YSSQI\",\"AvatarUrl\":\"https://i.pximg.net/user-profile/img/2019/01/13/21/00/08/15255001_2f78dcb00cc01551c55586280352571a_170.jpg\",\"Id\":\"17861677\",\"Account\":\"2653221698\",\"Password\":\"123456\",\"IsPremium\":false,\"Cookie\":\"__cf_bm=f0e58891ca78eb5120e98a615b93f4a19e54f3c7-1624823423-1800-AQxRUQM3QcHhmsgqRdPtxLgocDfsBVvnuKQXyJLVorilkCjFtF6X82EbDXtL33kGccmhFFBQ/K71zAOFvSxlz01jqk0XL1bzojG6DImc1RE8HVD/WC/YSquXeH0sJGPaeww31sInw1Ip7OjN1u4XcdXHqOWfwF3HNI1KR2S\\u002BBGEFn\\u002BcoiYpbIhkXh9lbXzbgfw==;b_type=1;privacy_policy_agreement=0;device_token=a799b12b2bae4cd02e715c273314030d;_gat=1;c_type=21;PHPSESSID=17861677_JQgKd8M21Nkbp82KCrBnsdbPK4Hxnvni;p_ab_id_2=8;_gid=GA1.2.253427416.1624744622;privacy_policy_notification=0;_ga=GA1.2.1567916307.1623632304;a_type=0;p_ab_d_id=626740158;d_type=1;p_ab_id=5;\",\"Bypass\":false,\"MirrorHost\":null,\"ExcludeTags\":null,\"IncludeTags\":null,\"MinBookmark\":0,\"AllowCache\":false}"
            .FromJson<Session>()!
            .UseBypass()
            .UseCache();

        private static readonly MakoClient MakoClient = new(Session, CultureInfo.CurrentCulture);

        private static readonly Action<JsonSerializerOptions> DefaultSerializerOptions = options =>
        {
            options.WriteIndented = true;
            options.IgnoreNullValues = true;
            options.ReferenceHandler = ReferenceHandler.Preserve;
        };
        
        private static async Task PrintIllusts(IAsyncEnumerable<Illustration> illustrations)
        {
            var cnt = 0;
            await foreach (var i in illustrations)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }
            
            System.Console.WriteLine($"Count: {cnt}");
        }
        
        private static async Task PrintUser(IAsyncEnumerable<User> users)
        {
            var cnt = 0;
            await foreach (var i in users)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }
        
        private static async Task PrintSpotlight(IAsyncEnumerable<SpotlightArticle> articles)
        {
            var cnt = 0;
            await foreach (var i in articles)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }
            
            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task PrintFeeds(IAsyncEnumerable<Feed> feeds)
        {
            var cnt = 0;
            await foreach (var i in feeds)
            {
                if (i != null)
                {
                    cnt++;
                    System.Console.WriteLine(i.ToJson(DefaultSerializerOptions));
                }
            }

            System.Console.WriteLine($"Count: {cnt}");
        }

        private static async Task Recommend()
        {
            var recommend = MakoClient.Recommends(RecommendContentType.Manga);
            await PrintIllusts(recommend);
        }
        
        private static async Task Ranking()
        {
            var ranking = MakoClient.Ranking(RankOption.Day, DateTime.Today - TimeSpan.FromDays(2));
            await PrintIllusts(ranking);
        }
        
        private static async Task GetBookmark()
        {
            var bookmarks = MakoClient.Bookmarks("7263576", PrivacyPolicy.Public);
            await PrintIllusts(bookmarks);
        }

        private static async Task Search()
        {
            var search = MakoClient.Search("東方project", pages: 6, sortOption: IllustrationSortOption.PopularityDescending);
            await PrintIllusts(search);
        }
        
        private static async Task RecommendIllustrators()
        {
            var illustrators = MakoClient.RecommendIllustrators();
            await PrintUser(illustrators);
        }
        
        private static async Task Spotlights()
        {
            var spotlights = MakoClient.Spotlights();
            await PrintSpotlight(spotlights);
        }

        private static async Task Feeds()
        {
            var feeds = MakoClient.Feeds();
            await PrintFeeds(feeds);
        }
        
        private static async Task Uploads()
        {
            var uploads = MakoClient.Uploads("333556");
            await PrintIllusts(uploads);
        }

        private static async Task Following()
        {
            var follows = MakoClient.Following("333556", PrivacyPolicy.Private);
            await PrintUser(follows);
        }

        private static async Task SearchUser()
        {
            var users = MakoClient.SearchUser("ideolo");
            await PrintUser(users);
        }
        
        private static async Task Updates()
        {
            var updates = MakoClient.Updates(PrivacyPolicy.Public);
            await PrintIllusts(updates);
        }

        private static async Task GetSpotlightIllustrations(string id)
        {
            var spotlightDetail = await MakoClient.GetSpotlightDetailAsync(id);
            await PrintIllusts(spotlightDetail!.Illustrations.ToAsyncEnumerable());
        }
        
        public static async Task Main()
        {
            await SearchUser();
        }
    }
}