using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage.Extensions
{
    public static class ArticlePayloadExtensions
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

            subject.Id.VerifyNotEmpty(nameof(subject.Id));
            subject.PackagePayload.VerifyAssert(x => x?.Length > 0, $"{nameof(subject.PackagePayload)} is required");
            subject.Hash.VerifyNotEmpty($"{nameof(subject.Hash)} is required");

            byte[] packagePayload = Convert.FromBase64String(subject.PackagePayload);
            byte[] hash = MD5.Create().ComputeHash(packagePayload);

            Convert.ToBase64String(hash).VerifyAssert(x => x == subject.Hash, "Hash verification failed");
        }

        public static bool IsValid(this ArticlePayload subject)
        {
            try
            {
                subject.Verify();
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static byte[] ToBytes(this ArticlePayload subject)
        {
            subject.Verify();

            return Convert.FromBase64String(subject.PackagePayload);
        }

        public static ArticlePayload ToArticlePayload(this byte[] subject)
        {
            subject.VerifyAssert(x => x?.Length > 0, $"{nameof(subject)} is empty");

            ArticleManifest articleManifest = subject.ReadManifest();
            return subject.ToArticlePayload((ArticleId)articleManifest.ArticleId);
        }

        public static ArticlePayload ToArticlePayload(this byte[] subject, ArticleId articleId)
        {
            subject.VerifyAssert(x => x?.Length > 0, $"{nameof(subject)} is empty");
            articleId.VerifyNotNull(nameof(articleId));

            var payload = new ArticlePayload
            {
                Id = (string)articleId,
                PackagePayload = Convert.ToBase64String(subject),
                Hash = Convert.ToBase64String(MD5.Create().ComputeHash(subject)),
            };

            payload.Verify();
            return payload;
        }

        public static ArticleManifest ReadManifest(this ArticlePayload subject) => subject.ToBytes().ReadManifest();

        public static byte[] GetPackageItem(this ArticlePayload subject, string path) => subject.ToBytes().GetPackageItem(path);

        public static byte[] GetPackageItem(this byte[] payload, string path)
        {
            using Stream payloadStream = new MemoryStream(payload);
            using var zipArchive = new ZipArchive(payloadStream, ZipArchiveMode.Read, false);

            ZipArchiveEntry? entry = zipArchive.GetEntry(path)
                .VerifyNotNull($"Could not find {path} manifest in article payload");

            using Stream fileStream = entry.Open();

            using MemoryStream memoryReader = new MemoryStream();
            fileStream.CopyTo(memoryReader);

            return memoryReader.ToArray();
        }

        public static IReadOnlyList<string> GetPackageEntries(this ArticlePayload subject) => subject.ToBytes().GetPackageEntries();

        public static IReadOnlyList<string> GetPackageEntries(this byte[] payload)
        {
            using Stream payloadStream = new MemoryStream(payload);
            using var zipArchive = new ZipArchive(payloadStream, ZipArchiveMode.Read, false);

            return zipArchive.Entries.Select(x => x.Name).ToList();
        }
    }
}