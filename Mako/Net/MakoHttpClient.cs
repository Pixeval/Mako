// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Net.Http;

namespace Mako.Net;

internal class MakoHttpClient(HttpMessageHandler handler) : HttpClient(handler);
