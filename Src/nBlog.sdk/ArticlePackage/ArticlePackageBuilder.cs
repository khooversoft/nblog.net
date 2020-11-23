using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Toolbox.Extensions;
using Toolbox.Model;
using Toolbox.Models;
using Toolbox.Services;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public class ArticlePackageBuilder
    {
        private string _specFile = null!;
        private string _specFileBase = null!;
        private string _buildFolder = null!;
        private string _locationFolder = null!;

        public static string ManifestFileName { get; } = "articlePackage.manifest.json";

        public ArticleSpec ArticleSpec { get; private set; } = null!;

        public ArticlePackageBuilder SetSpecFile(string specFile)
        {
            specFile.VerifyNotEmpty(nameof(specFile));

            _specFile = specFile;

            _specFileBase = Path.GetDirectoryName(Path.GetFullPath(specFile))
                .VerifyNotEmpty("Folder is required, cannot be on the root of the drive");

            return this;
        }

        public ArticlePackageBuilder SetBuildFolder(string buildFolder) => this.Action(x => _buildFolder = buildFolder);

        public ArticlePackageBuilder SetLocationFolder(string locationFolder) => this.Action(x => _locationFolder = locationFolder);

        public int Build(Action<FileActionProgress>? monitor = null, CancellationToken token = default)
        {
            _specFile.VerifyNotEmpty("Specification file not read");
            _specFileBase.VerifyNotEmpty("Specification file not read");
            _buildFolder.VerifyNotEmpty("Deployment folder not specified");
            _locationFolder.VerifyNotEmpty("Location folder not specified");

            ArticleSpec = ReadPackageSpec(_specFile);

            ArticleSpec
                .VerifyNotNull("Option is required")
                .Verify();

            CopyTo[] files = GetFiles()
                .Append(WriteManifest())
                .ToArray();

            string zipFilePath = Path.Combine(_buildFolder, _locationFolder, ArticleSpec.PackageFile);

            Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);

            new ZipFile(zipFilePath).CompressFiles(token, monitor, files);

            return files.Length;
        }

        private CopyTo[] GetFiles()
        {
            var baseValues = ArticleSpec.Copy
                .Select(x => (Source: Path.Combine(_specFileBase, x.Source), x.Destination))
                .Select(x => (x.Source, basePath: Path.GetDirectoryName(x.Source)!, search: Path.GetFileName(x.Source), destination: x.Destination, wildcard: x.Source.IndexOf('*') >= 0));

            CopyTo[] files = baseValues
                .SelectMany(
                    x => Directory.GetFiles(x.basePath, x.search, x.wildcard ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
                    (x, c) => new CopyTo { Source = c, Destination = !x.wildcard ? x.destination : Path.Combine(x.destination, c.Substring(x.basePath.Length + 1)) }
                    )
                .ToArray();

            // Verify any non-wild card files are in the list
            baseValues
                .Where(x => x.wildcard == false && !files.Any(y => x.Source.IndexOf(y.Source) >= 0))
                .Select(x => x.Source)
                .VerifyAssert<IEnumerable<string>, FileNotFoundException>(x => x.Count() == 0, x => $"File required where not found, {string.Join(", ", x)}");

            return files;
        }

        public ArticleSpec ReadPackageSpec(string filePath)
        {
            filePath
                .VerifyNotEmpty(nameof(filePath))
                .VerifyAssert(x => File.Exists(x), $"{filePath} does not exist");

            string subject = File.ReadAllText(filePath)
                .VerifyNotEmpty($"{filePath} is empty");

            ArticleSpec spec = Json.Default.Deserialize<ArticleSpec>(subject)
                .VerifyNotNull($"File {filePath} is not a valid spec format");

            spec.Verify();

            IPropertyResolver resolver = GetResolver();

            return new ArticleSpec
            {
                PackageFile = resolver.Resolve(spec.PackageFile),

                Manifest = new ArticleManifest
                {
                    ArticleId = resolver.Resolve(spec.Manifest.ArticleId),
                    PackageVersion = resolver.Resolve(spec.Manifest.PackageVersion),
                    Title = resolver.Resolve(spec.Manifest.Title),
                    Author = resolver.Resolve(spec.Manifest.Author),
                    Tags = spec.Manifest?.Tags?.Select(x => resolver.Resolve(x))?.ToList(),
                },

                Copy = spec.Copy.Select(x => new CopyTo
                {
                    Source = resolver.Resolve(x.Source),
                    Destination = resolver.Resolve(x.Destination),
                }).ToList(),
            };
        }

        private CopyTo WriteManifest()
        {
            string filePath = Path.Combine(Path.GetTempPath(), ManifestFileName);
            ArticleSpec.Manifest.WriteToFile(filePath);

            return new CopyTo
            {
                Source = filePath,
                Destination = ManifestFileName,
            };
        }

        private IPropertyResolver GetResolver()
        {
            return new (string Key, string Value)[]
            {
                ("specFile", Path.GetFileNameWithoutExtension(_specFile)),
                ("locationFolder", _locationFolder),
            }
            .Select(x => new KeyValuePair<string, string>(x.Key, x.Value))
            .Func(x => new PropertyResolver(x));
        }
    }
}
