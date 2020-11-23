using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static void Verify(this ArticleManifest subject)
        {
            subject.VerifyNotNull(nameof(subject));

            subject.ArticleId.VerifyNotEmpty($"{nameof(subject.ArticleId)} is required");
            subject.PackageVersion.VerifyNotEmpty($"{nameof(subject.PackageVersion)} is required");
            subject.Title.VerifyNotEmpty($"{nameof(subject.Title)} is required");
        }

        public static void WriteToFile(this ArticleManifest mlPackageManifest, string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            mlPackageManifest.Verify();
            string json = Json.Default.SerializeFormat(mlPackageManifest);
            File.WriteAllText(filePath, json);
        }
    }
}
