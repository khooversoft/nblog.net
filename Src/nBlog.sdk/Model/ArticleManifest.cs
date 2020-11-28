﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record ArticleManifest
    {
        private string _articleId = null!;

        public string ArticleId
        {
            get => _articleId;
            init => _articleId = Model.ArticleId.ConvertTo(value);
        }

        public string PackageVersion { get; init; } = "1.0.0.0";

        public string Title { get; init; } = null!;

        public string? Author { get; init; }

        public DateTime Date { get; init; }

        public IList<string>? Tags { get; init; }
    }
}