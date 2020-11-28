using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public static class ArticleExtensions
    {
        public static void Verify(this ArticleSpec subject)
        {
            subject.VerifyNotNull(nameof(subject));

            subject.PackageFile.VerifyNotEmpty($"{nameof(subject.PackageFile)} is required");
            subject.Manifest.VerifyNotNull($"{nameof(subject.Manifest)} is required");

            subject.Copy
                .VerifyNotNull($"{nameof(subject.Copy)} is required")
                .VerifyAssert(x => x.Count > 0, $"{nameof(subject.Copy)} has is empty");
        }

        public static void Verify(this ArticlePayload subject)
        {
            subject.VerifyNotNull(nameof(subject));

            subject.PackagePayload.VerifyAssert(x => x?.Length > 0, $"{nameof(subject.PackagePayload)} is required");
            subject.Hash.VerifyNotEmpty($"{nameof(subject.Hash)} is required");

            byte[] packagePayload = Convert.FromBase64String(subject.PackagePayload);
            byte[] hash = MD5.Create().ComputeHash(packagePayload);

            Convert.ToBase64String(hash).VerifyAssert(x => x == subject.Hash, "Hash verification failed");
        }

        public static byte[] ToBytes(this ArticlePayload subject)
        {
            subject.Verify();

            return Convert.FromBase64String(subject.PackagePayload);
        }

        public static ArticlePayload ToArticlePayload(this byte[] subject, string id)
        {
            subject.VerifyAssert(x => x?.Length > 0, $"{nameof(subject)} is empty");
            id.VerifyNotEmpty(nameof(id));

            var payload = new ArticlePayload
            {
                Id = id,
                PackagePayload = Convert.ToBase64String(subject),
                Hash = Convert.ToBase64String(MD5.Create().ComputeHash(subject)),
            };

            payload.Verify();
            return payload;
        }

        public static ArticleManifest ReadManifest(this ArticlePayload subject)
        {
            byte[] payload = subject.ToBytes();

            using Stream payloadStream = new MemoryStream(payload);
            using var zipArchive = new ZipArchive(payloadStream, ZipArchiveMode.Read, false);

            ZipArchiveEntry? entry = zipArchive.GetEntry(ArticlePackageBuilder.ManifestFileName)
                .VerifyNotNull($"Could not find {ArticlePackageBuilder.ManifestFileName} manifest in article payload");

            using Stream fileStream = entry.Open();
            using StreamReader memoryReader = new StreamReader(fileStream);

            string json = memoryReader.ReadToEnd();

            return Json.Default.Deserialize<ArticleManifest>(json)
                .VerifyNotNull("Cannot deserialize from package");
        }
    }
}
