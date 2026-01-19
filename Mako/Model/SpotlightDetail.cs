// Copyright (c) Mako.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Mako.Model;

public record SpotlightDetail(Spotlight SpotlightArticle, string Introduction, IEnumerable<Illustration> Illustrations);
