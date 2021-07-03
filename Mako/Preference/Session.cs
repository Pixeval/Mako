#region Copyright (c) Pixeval/Mako

// MIT License
// 
// Copyright (c) Pixeval 2021 Mako/Session.cs
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
using JetBrains.Annotations;
using Mako.Util;

namespace Mako.Preference
{
    /// <summary>
    ///     Contains all the user configurable
    /// </summary>
    [PublicAPI]
    public record Session
    {
        /// <summary>
        ///     User name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Token expiration
        /// </summary>
        public DateTimeOffset ExpireIn { get; set; }

        /// <summary>
        ///     Current access token
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        ///     Current refresh token
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        ///     Avatar
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        ///     User id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        ///     Account for login
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        ///     Indicates current user is Pixiv Premium or not
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        ///     WebAPI cookie
        /// </summary>
        public string? Cookie { get; set; }

        public override string? ToString()
        {
            return this.ToJson();
        }

        public bool RefreshRequired()
        {
            return AccessToken.IsNullOrEmpty() || DateTime.Now >= ExpireIn;
        }
    }
}