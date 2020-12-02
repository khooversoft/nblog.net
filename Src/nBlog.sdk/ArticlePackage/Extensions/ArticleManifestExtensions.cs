using nBlog.sdk.Model;
using System.IO;
using System.IO.Compression;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage.Extensions
{
    public static class ArticleManifestExtensions
    {
        public static void Verify(this ArticleManifest subject)
        {
            subject.VerifyNotNull(nameof(subject));

            subject.ArticleId.VerifyNotEmpty($"{nameof(subject.ArticleId)} is required");
            ArticleId.VerifyId(subject.ArticleId);

            subject.PackageVersion.VerifyNotEmpty($"{nameof(subject.PackageVersion)} is required");
            subject.Title.VerifyNotEmpty($"{nameof(subject.Title)} is required");
        }

        public static void WriteToFile(this ArticleManifest mlPackageManifest, string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            mlPackageManifest.Verify();
            var json = Json.Default.SerializeFormat(mlPackageManifest);
            File.WriteAllText(filePath, json);
        }

        public static ArticleManifest ReadManifest(this byte[] payload)
        {
            using Stream payloadStream = new MemoryStream(payload);
            using var zipArchive = new ZipArchive(payloadStream, ZipArchiveMode.Read, false);

            ZipArchiveEntry? entry = zipArchive.GetEntry(ArticleConstants.ManifestFileName)
                .VerifyNotNull($"Could not find {ArticleConstants.ManifestFileName} manifest in article payload");

            using Stream fileStream = entry.Open();
            using StreamReader memoryReader = new StreamReader(fileStream);

            string json = memoryReader.ReadToEnd();

            return Json.Default.Deserialize<ArticleManifest>(json)
                .VerifyNotNull("Cannot deserialize from package");
        }
    }
}