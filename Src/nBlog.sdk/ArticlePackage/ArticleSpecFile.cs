using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Model;
using Toolbox.Services;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public class ArticleSpecFile
    {
        private readonly string _file;

        public ArticleSpecFile(string file) => _file = file.VerifyNotEmpty(nameof(file));

        public ArticleSpec Read()
        {
            _file.VerifyAssert(x => File.Exists(x), $"{_file} does not exist");

            string json = File.ReadAllText(_file)
                .VerifyNotEmpty($"{_file} is empty");

            ArticleSpec spec = Json.Default.Deserialize<ArticleSpec>(json)
                .VerifyNotNull($"File {_file} is not a valid spec format");

            spec.Verify();

            IPropertyResolver resolver = GetResolver(spec);

            return new ArticleSpec
            {
                PackageFile = resolver.Resolve(spec.PackageFile)
                    .Replace(" ", ".")
                    .Replace("/", "."),

                Manifest = new ArticleManifest
                {
                    ArticleId = resolver.Resolve(spec.Manifest.ArticleId),
                    PackageVersion = resolver.Resolve(spec.Manifest.PackageVersion),
                    Title = resolver.Resolve(spec.Manifest.Title),
                    Author = resolver.Resolve(spec.Manifest.Author),
                    Date = spec.Manifest.Date,
                    Tags = spec.Manifest?.Tags?.Select(x => resolver.Resolve(x))?.ToList(),
                },

                Copy = spec.Copy.Select(x => new CopyTo
                {
                    Source = resolver.Resolve(x.Source),
                    Destination = resolver.Resolve(x.Destination),
                }).ToList(),
            };
        }

        private IPropertyResolver GetResolver(ArticleSpec spec)
        {
            return new[]
            {
                new KeyValuePair<string, string>("specFile", Path.GetFileNameWithoutExtension(_file)),
                new KeyValuePair<string, string>("articleId", spec.Manifest.ArticleId),
            }
            .Func(x => new PropertyResolver(x));
        }
    }
}
