﻿#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako.Authenticator/MainWindow.xaml.cs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using Mako.Model;
using Mako.Util;
using Microsoft.Web.WebView2.Core;

namespace Mako.Authenticator
{
    public partial class MainWindow
    {
        private readonly TaskCompletionSource<(string, string)> _webViewLoginCompletion = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private static byte[] HashBytes<T>(string str, Encoding? encoding = null) where T : HashAlgorithm, new()
        {
            using var crypt = new T();
            var hashBytes = crypt.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(str));
            return hashBytes;
        }

        private static string GetCodeChallenge(string code)
        {
            return ToUrlSafeBase64String(HashBytes<SHA256CryptoServiceProvider>(code, Encoding.ASCII));
        }

        private static string GetCodeVerify()
        {
            var bytes = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return ToUrlSafeBase64String(bytes);
        }

        private static string GenerateWebPageUrl(string codeVerify, bool signUp = false)
        {
            var codeChallenge = GetCodeChallenge(codeVerify);
            return signUp
                ? $"https://app-api.pixiv.net/web/v1/provisional-accounts/create?code_challenge={codeChallenge}&code_challenge_method=S256&client=pixiv-android"
                : $"https://app-api.pixiv.net/web/v1/login?code_challenge={codeChallenge}&code_challenge_method=S256&client=pixiv-android";
        }

        private static string ToUrlSafeBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd(new[] {'='}).Replace("+", "-").Replace("/", "_");
        }

        private static string AsString(IEnumerable<CoreWebView2Cookie> cookies)
        {
            return cookies.Aggregate("", (s, cookie) => s + $"{cookie.Name}={cookie.Value};");
        }

        private async void LoginWebView_OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("pixiv://"))
            {
                _webViewLoginCompletion.SetResult((e.Uri, AsString(await LoginWebView.CoreWebView2.CookieManager.GetCookiesAsync("https://www.pixiv.net"))));
            }
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoginWebView.EnsureCoreWebView2Async();
            var verifier = GetCodeVerify();
            LoginWebView.Source = new Uri(GenerateWebPageUrl(verifier));

            var (url, cookie) = await _webViewLoginCompletion.Task;
            var code = HttpUtility.ParseQueryString(new Uri(url).Query)["code"];

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var responseMessage = await httpClient.PostAsync("https://oauth.secure.pixiv.net/auth/token", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code!),
                new KeyValuePair<string, string>("code_verifier", verifier),
                new KeyValuePair<string, string>("client_id", "MOBrBDS8blbauoSck0ZfDbtuzpyT"),
                new KeyValuePair<string, string>("client_secret", "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj"),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("include_policy", "true"),
                new KeyValuePair<string, string>("redirect_uri", "https://app-api.pixiv.net/web/v1/users/auth/pixiv/callback")
            }!));
            var session = (await responseMessage.Content.ReadAsStringAsync())
                .FromJson<TokenResponse>()!
                .ToSession() with
                {
                    Cookie = cookie,
                    CookieCreation = DateTimeOffset.Now
                };
            Clipboard.SetText((await session!.ToJsonAsync())!);
            Close();
        }
    }
}